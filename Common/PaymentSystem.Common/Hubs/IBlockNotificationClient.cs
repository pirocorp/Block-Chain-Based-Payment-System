namespace PaymentSystem.Common.Hubs
{
    using System.Threading.Tasks;

    using PaymentSystem.Common.Hubs.Models;

    public interface IBlockNotificationClient
    {
        Task ReceiveBlock(NotificationBlock block);
    }
}
