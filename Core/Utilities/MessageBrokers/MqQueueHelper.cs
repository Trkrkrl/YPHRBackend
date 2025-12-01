using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Core.Utilities.Mail;

namespace Core.Utilities.MessageBrokers
{
    /// <summary>
    /// Kuyruğa mesaj ve email job'ları atan helper sınıfı.
    /// MessageBrokerOptions üzerinden host / queue gibi bilgiler appsettings'ten gelir.
    /// </summary>
    public class MqQueueHelper : IMessageBrokerHelper
    {
        private readonly MessageBrokerOptions _brokerOptions;

        public IConfiguration Configuration { get; }

        public MqQueueHelper(IConfiguration configuration, IOptions<MessageBrokerOptions> options)
        {
            Configuration  = configuration;
            _brokerOptions = options.Value;
        }

        public async Task QueueMessageAsync(string messageText, CancellationToken ct = default)
        {
            var factory = CreateFactory();

            await using var connection = await factory.CreateConnectionAsync(ct);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

            await channel.QueueDeclareAsync(
                queue: _brokerOptions.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct);

            var payload = JsonConvert.SerializeObject(messageText);
            var body    = Encoding.UTF8.GetBytes(payload);

            var props = new BasicProperties { ContentType = "application/json" };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _brokerOptions.QueueName,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: ct);
        }

        public async Task QueueEmailAsync(EmailMessage emailMessage, CancellationToken ct = default)
        {
            var factory = CreateFactory();

            await using var connection = await factory.CreateConnectionAsync(ct);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

            var queueName = string.IsNullOrWhiteSpace(_brokerOptions.EmailQueueName)
                ? _brokerOptions.QueueName
                : _brokerOptions.EmailQueueName;

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct);

            var payload = JsonConvert.SerializeObject(emailMessage);
            var body    = Encoding.UTF8.GetBytes(payload);

            var props = new BasicProperties { ContentType = "application/json" };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: ct);
        }

        // Eski sync imzaları korumak için wrapper'lar
        public void QueueMessage(string messageText) =>
            QueueMessageAsync(messageText).GetAwaiter().GetResult();

        public void QueueEmail(EmailMessage emailMessage) =>
            QueueEmailAsync(emailMessage).GetAwaiter().GetResult();

        private ConnectionFactory CreateFactory() => new ConnectionFactory
        {
            HostName    = _brokerOptions.HostName,
            UserName    = _brokerOptions.UserName,
            Password    = _brokerOptions.Password,
            Port        = _brokerOptions.Port,
            VirtualHost = _brokerOptions.VirtualHost
        };
    }
}
