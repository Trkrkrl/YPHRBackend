namespace Core.Utilities.Mail
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Gönderici görünen adı ("DisplayName") appsettings'ten de okunabilir.
        public string DisplayName { get; set; } = string.Empty;
    }
}
