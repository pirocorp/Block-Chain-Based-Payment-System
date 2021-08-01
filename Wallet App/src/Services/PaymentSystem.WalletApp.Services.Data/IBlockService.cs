namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Hubs.Models;

    public interface IBlockService
    {
        Task<Block> GetLastBlock();

        public Task ReceiveBlock(Block notificationBlock, IEnumerable<CanceledTransaction> canceledTransactions);
    }
}
