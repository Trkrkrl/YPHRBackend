using Core.Utilities.Mail;

namespace Core.Utilities.MessageBrokers
{
    public interface IMessageBrokerHelper
    {
        void QueueMessage(string messageText);
        void QueueEmail(EmailMessage email);
    }
}
