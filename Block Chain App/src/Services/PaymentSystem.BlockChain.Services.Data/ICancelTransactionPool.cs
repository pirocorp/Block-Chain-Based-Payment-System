namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Collections.Generic;

    using PaymentSystem.Common.Data.Models;

    public interface ICancelTransactionPool
    {
        void Add(Transaction transaction);

        IEnumerable<Transaction> GetAll();

        bool Exists(string hash);
    }
}
