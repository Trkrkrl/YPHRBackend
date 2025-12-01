using System.Collections.Generic;

namespace Core.Utilities.Mail
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            ToAddresses = new List<EmailAddress>();
            FromAddresses = new List<EmailAddress>();
        }

        public List<EmailAddress> ToAddresses { get; set; }
        public List<EmailAddress> FromAddresses { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int TryCount { get; set; } = 0;
        public string Status { get; set; } = "sending"; // 5 kez gönderilemezse "failed" yapılabilir.
    }
}
