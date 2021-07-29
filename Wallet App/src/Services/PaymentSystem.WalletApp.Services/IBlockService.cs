namespace PaymentSystem.WalletApp.Services
{
    using System.Threading.Tasks;

    using PaymentSystem.Common.Hubs.Models;

    public interface IBlockService
    {
        public Task ReceiveBlock(NotificationBlock block);
    }
}
