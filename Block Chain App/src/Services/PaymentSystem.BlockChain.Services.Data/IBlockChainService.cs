namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.Common.Data.Models;

    public interface IBlockChainService
    {
        Task<int> Count();

        Task<Block> GetLastBlock();

        Task<Block> GetGenesisBlock();

        Task<Block> GetBlockByHash(string hash);

        Task<Block> GetBlockByHeight(long height);

        Task<IEnumerable<Block>> GetBlocks(int pageNumber, int resultPerPage);

        void AddTransaction(Transaction transaction);

        IAsyncEnumerable<Block[]> GetBlockChain(long height);

        Task MineBlock();
    }
}
