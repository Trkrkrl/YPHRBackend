namespace Core.Utilities.MessageBrokers
{
    /// <summary>
    /// Mesaj kuyruğu yapılandırma seçenekleri.
    /// appsettings.json örneği (WebAPI tarafında eklenecek):
    ///
    /// "MessageBrokerOptions": {
    ///   "Provider": "RabbitMq",
    ///   "HostName": "rabbitmq",
    ///   "Port": 5672,
    ///   "UserName": "guest",
    ///   "Password": "guest",
    ///   "VirtualHost": "/",
    ///   "QueueName": "DArchQueue",
    ///   "EmailQueueName": "DArchEmailQueue"
    /// }
    ///
    /// Local / docker / production ortamları için farklı değerler tanımlanabilir.
    /// </summary>
    public class MessageBrokerOptions
    {
        public string Provider { get; set; } = "RabbitMq"; // İleride farklı provider destekleri için.
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string QueueName { get; set; } = "DArchQueue";
        public string EmailQueueName { get; set; } = "DArchEmailQueue";
        public bool  Enabled{ get; set; }=false;
    }
}
