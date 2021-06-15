namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using PaymentSystem.Common.Data.Models;

    public class TransactionPool : ITransactionPool
    {
        private readonly IDictionary<string, Transaction> pool;

        public TransactionPool()
        {
            this.pool = new ConcurrentDictionary<string, Transaction>();
        }

        public bool Exists(string hash)
            => this.pool.ContainsKey(hash);

        public void AddTransaction(Transaction transaction)
        {
            this.pool.Add(transaction.Hash, transaction);
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            var transactions = this.pool.Values.ToList();
            this.pool.Clear();

            return transactions;
        }
    }
}
