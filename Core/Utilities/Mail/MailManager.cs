using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;
using Core.CrossCuttingConcerns.Logging.Serilog.Logger;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Logger;

namespace Core.Utilities.Mail
{
    public class MailManager : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Email gönderimini gerçekleştirir. LogAspect ile dosyaya loglanır.
        /// appsettings.json örneği WebAPI tarafında eklenecek:
        /// "EmailConfiguration": { "Host": "...", "Port": 587, "Mail": "...", "Password": "...", "DisplayName": "..." }
        /// </summary>
        [LogAspect(typeof(FileLogger))]
        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var emailSection = _configuration.GetSection("EmailConfiguration");

            var fromAddress = emailSection.GetSection("Mail").Value;
            var displayName = emailSection.GetSection("DisplayName").Value;

            emailMessage.FromAddresses.Add(new EmailAddress
            {
                Address = fromAddress ?? string.Empty,
                Name    = displayName ?? string.Empty
            });

            var message = new MimeMessage();

            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.Subject = emailMessage.Subject;

            // İçerik HTML olarak gönderiliyor.
            var messageBody = emailMessage.Content;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = messageBody
            };

            using var emailClient = new SmtpClient();

            var host     = emailSection.GetSection("Host").Value;
            var portText = emailSection.GetSection("Port").Value;
            var mailUser = emailSection.GetSection("Mail").Value;
            var password = emailSection.GetSection("Password").Value;

            int port = 25;
            if (!string.IsNullOrWhiteSpace(portText))
            {
                int.TryParse(portText, out port);
            }

            await emailClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);

            if (!string.IsNullOrWhiteSpace(mailUser))
            {
                await emailClient.AuthenticateAsync(mailUser, password);
            }

            await emailClient.SendAsync(message);
            await emailClient.DisconnectAsync(true);
        }
    }
}
