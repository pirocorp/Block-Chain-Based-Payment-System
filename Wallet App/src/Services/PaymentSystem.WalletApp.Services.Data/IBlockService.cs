namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Hubs.Models;

    public interface IBlockService
    {
        Task<Block> GetLastBlock();

        Task<IEnumerable<T>> GetLatestBlocks<T>();

        Task ReceiveBlock(Block notificationBlock, IEnumerable<CanceledTransaction> canceledTransactions);

        Task<(int Total, IEnumerable<T> Blocks)> GetBlocks<T>(int page, int currentPageSize);
    }
}
