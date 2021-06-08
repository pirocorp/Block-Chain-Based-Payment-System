namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using PaymentSystem.Common.Data.Models;

    public class TransactionPool : ITransactionPool
    {
        private readonly ConcurrentBag<Transaction> pool;

        public TransactionPool()
        {
            this.pool = new ConcurrentBag<Transaction>();
        }

        public void AddTransaction(Transaction transaction)
        {
            this.pool.Add(transaction);
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            var transactions = this.pool.ToArray();
            this.pool.Clear();

            return transactions;
        }
    }
}
