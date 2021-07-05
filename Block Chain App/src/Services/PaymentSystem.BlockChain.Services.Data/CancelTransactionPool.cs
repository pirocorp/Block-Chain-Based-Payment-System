namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Collections.Generic;

    using PaymentSystem.BlockChain.Services.Data.Pool;
    using PaymentSystem.Common.Data.Models;

    public class CancelTransactionPool : ICancelTransactionPool
    {
        private readonly IPool<Transaction> pool;

        public void Add(Transaction transaction)
            => this.pool.Add(transaction.Hash, transaction);

        public IEnumerable<Transaction> GetAll()
            => this.pool.Get();

        public bool Exists(string hash)
            => this.pool.Exists(hash);
    }
}
