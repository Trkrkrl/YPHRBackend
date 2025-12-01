using System.Threading.Tasks;

namespace Core.Utilities.MessageBrokers
{
    /// <summary>
    /// Mesaj kuyruğundan tüketim yapan consumer arabirimi.
    /// İleride farklı consumer implementasyonları için genişletilebilir.
    /// </summary>
    public interface IMessageConsumer
    {
        Task GetQueue();
    }
}
