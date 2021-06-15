namespace PaymentSystem.Common.Hubs
{
    using System.Threading.Tasks;

    using PaymentSystem.Common.Hubs.Models;

    public interface IBroadcastClient
    {
        Task ReceiveBlock(NotificationBlock block);
    }
}
