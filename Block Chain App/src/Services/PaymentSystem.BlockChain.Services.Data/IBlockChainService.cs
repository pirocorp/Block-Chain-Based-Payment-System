namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Collections.Generic;

    using PaymentSystem.Common.Data.Models;

    public interface IBlockChainService
    {
        int Count();

        Block GetLastBlock();

        Block GetGenesisBlock();

        Block GetBlockByHash(string hash);

        Block GetBlockByHeight(long height);

        IEnumerable<Block> GetBlocks(int pageNumber, int resultPerPage);

        void AddTransaction(Transaction transaction);

        IEnumerable<IEnumerable<Block>> GetBlockChain(long height);

        void MineBlock();
    }
}
