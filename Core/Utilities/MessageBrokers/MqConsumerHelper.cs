using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Core.Utilities.Mail;

namespace Core.Utilities.MessageBrokers
{
    /// <summary>
    /// RabbitMQ kuyruğundan email job'larını tüketen BackgroundService.
    /// Retry mantığı ile belirli sayıda deneyip başarısız olan mailleri 'failed' durumuna çeker.
    /// .NET 8 + RabbitMQ.Client 7.x için tam asenkron tasarlanmıştır.
    /// </summary>
    public sealed class MqConsumerHelper : BackgroundService, IMessageConsumer, IAsyncDisposable
    {
        private const int MaxRetryCount = 5;

        private IConnection? _connection;
        private IChannel? _channel;

        private readonly MessageBrokerOptions _brokerOptions;
        private readonly IMailService _mailService;
        private readonly IMessageBrokerHelper _messageBrokerHelper;

        // BackgroundService içindeki stoppingToken'a EventHandler'lardan erişebilmek için
        private CancellationToken _stoppingToken;

        private string HostName => _brokerOptions.HostName;
        private string QueueName => _brokerOptions.QueueName;

        private string EmailQueueName =>
            string.IsNullOrWhiteSpace(_brokerOptions.EmailQueueName)
                ? _brokerOptions.QueueName
                : _brokerOptions.EmailQueueName;

        public MqConsumerHelper(
            IConfiguration configuration,
            IMailService mailService,
            IOptions<MessageBrokerOptions> brokerOptions,
            IMessageBrokerHelper messageBrokerHelper)
        {
            // Şimdilik kullanılmıyor ama DI imzasını bozmamak için tutalım
            _ = configuration;

            _brokerOptions       = brokerOptions.Value;
            _mailService         = mailService;
            _messageBrokerHelper = messageBrokerHelper;
        }

        /// <summary>
        /// BackgroundService tetiklendiğinde email kuyruğunu dinlemeye başlar.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            _stoppingToken = stoppingToken;

            // RabbitMQ bağlantısını ve channel'ı async olarak başlat
            await InitializeRabbitMqListenerAsync(stoppingToken).ConfigureAwait(false);

            // Email kuyruğu consumer'ını başlat
            await StartEmailConsumerAsync(stoppingToken).ConfigureAwait(false);

            // Host durdurulana kadar servisi canlı tut
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // uygulama kapanıyor, beklenen durum
            }
        }

        /// <summary>
        /// RabbitMQ bağlantısını asenkron olarak başlatır.
        /// </summary>
        private async Task InitializeRabbitMqListenerAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName    = _brokerOptions.HostName,
                UserName    = _brokerOptions.UserName,
                Password    = _brokerOptions.Password,
                Port        = _brokerOptions.Port,
                VirtualHost = _brokerOptions.VirtualHost,

                // Otomatik connection recovery önerilir
                AutomaticRecoveryEnabled = true,

                // İstersen consumer dispatch concurrency ile paralelliği artırabilirsin.
                // ConsumerDispatchConcurrency = 1
            };

            // RabbitMQ.Client 7.x: async connection API
            _connection = await factory.CreateConnectionAsync(
                clientProvidedName: "st10-email-consumer",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            _connection.ConnectionShutdownAsync += RabbitMq_ConnectionShutdownAsync;

            // Async channel API (IChannel)
            _channel = await _connection.CreateChannelAsync(
                cancellationToken: cancellationToken).ConfigureAwait(false);

            // Consumer tarafında kuyruğu declare etmek idempotent ve yaygın pratiktir
            await _channel.QueueDeclareAsync(
                queue: EmailQueueName,
                durable: true,   // PROD için kalıcı olması genelde daha güvenli
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            // Consumer başına aynı anda 1 mesaj (back-pressure)
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            _channel.ChannelShutdownAsync += OnChannelShutdownAsync;
        }

        /// <summary>
        /// IMessageConsumer generic kuyruk tüketimi. Şimdilik email kuyruğu ile aynı.
        /// </summary>
        public Task GetQueue() => StartEmailConsumerAsync(_stoppingToken);

        /// <summary>
        /// Email kuyruğunu dinler, gelen EmailMessage nesnelerini handle eder.
        /// </summary>
        private Task StartEmailConsumerAsync(CancellationToken cancellationToken)
        {
            if (_channel is null)
            {
                throw new InvalidOperationException("RabbitMQ channel is not initialized. Call InitializeRabbitMqListenerAsync first.");
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync     += OnMessageReceivedAsync;
            consumer.ShutdownAsync     += OnConsumerShutdownAsync;
            consumer.RegisteredAsync   += OnConsumerRegisteredAsync;
            consumer.UnregisteredAsync += OnConsumerUnregisteredAsync;

            // Consumer'ı kuyruğa bağla (async)
            return _channel.BasicConsumeAsync(
                queue: EmailQueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bir mesaj geldiğinde tetiklenen handler.
        /// </summary>
        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs mq)
        {
            if (_channel is null)
            {
                // Çok edge bir durum: channel yoksa Nack + requeue edilebilir
                return;
            }

            try
            {
                _stoppingToken.ThrowIfCancellationRequested();

                var body  = mq.Body.ToArray();
                var json  = Encoding.UTF8.GetString(body);
                var email = JsonSerializer.Deserialize<EmailMessage>(json);

                if (email is not null)
                {
                    await HandleMailAsync(email, _stoppingToken).ConfigureAwait(false);
                }

                // Başarılı işleme sonrası ACK
                await _channel.BasicAckAsync(
                    deliveryTag: mq.DeliveryTag,
                    multiple: false,
                    cancellationToken: _stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Uygulama kapanırken burada olabiliriz: isteğe göre NACK + requeue yapılabilir
            }
            catch
            {
                // Burada loglama yapılması tavsiye edilir.
                // Retry stratejisi HandleMailAsync içinde uygulanıyor.
            }
        }

        /// <summary>
        /// Mail gönderimini retry mantığı ile yönetir.
        /// </summary>
        private async Task HandleMailAsync(EmailMessage email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nextTry = email.TryCount + 1;

            if (nextTry > MaxRetryCount)
            {
                email.Status = "failed";
                return;
            }

            email.TryCount = nextTry;

            try
            {
                email.Status = "sending";

                // IMailService içinde CancellationToken destekliyorsan buraya ekleyebilirsin.
                await _mailService.SendEmailAsync(email).ConfigureAwait(false);

                email.Status = "sent";
            }
            catch
            {
                if (email.TryCount >= MaxRetryCount)
                {
                    email.Status = "failed";
                    // Burada istersen dead-letter queue'ye publish edebilirsin.
                }
                else
                {
                    // Retry için tekrar kuyruğa gönder
                    email.Status = "sending";
                    _messageBrokerHelper.QueueEmail(email);
                }
            }
        }

        #region Consumer / Connection / Channel Events

        private Task OnConsumerShutdownAsync(object sender, ShutdownEventArgs e)
            => Task.CompletedTask;

        private Task OnConsumerRegisteredAsync(object sender, ConsumerEventArgs e)
            => Task.CompletedTask;

        private Task OnConsumerUnregisteredAsync(object sender, ConsumerEventArgs e)
            => Task.CompletedTask;

        private Task OnChannelShutdownAsync(object sender, ShutdownEventArgs e)
            => Task.CompletedTask;

        private Task RabbitMq_ConnectionShutdownAsync(object sender, ShutdownEventArgs e)
            => Task.CompletedTask;

        #endregion

        /// <summary>
        /// IAsyncDisposable implementasyonu – bağlantı ve channel async kapatılır.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
            {
                try
                {
                    await _channel.CloseAsync().ConfigureAwait(false);
                }
                catch
                {
                    // Shutdown sırasında oluşan hataları yut
                }

                await _channel.DisposeAsync().ConfigureAwait(false);
                _channel = null;
            }

            if (_connection is not null)
            {
                try
                {
                    await _connection.CloseAsync().ConfigureAwait(false);
                }
                catch
                {
                    // Shutdown sırasında oluşan hataları yut
                }

                await _connection.DisposeAsync().ConfigureAwait(false);
                _connection = null;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sync dispose isteyen yerler için async dispose'u bloklayarak çağırıyoruz.
        /// </summary>
        public override void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
            base.Dispose();
        }
    }
}
