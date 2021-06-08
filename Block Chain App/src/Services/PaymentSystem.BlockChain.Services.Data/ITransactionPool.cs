namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Collections.Generic;

    using PaymentSystem.Common.Data.Models;

    /// <summary>
    /// All pending transactions are stored here.
    /// </summary>
    public interface ITransactionPool
    {
        /// <summary>
        /// Add new transaction to the pool.
        /// </summary>
        /// <param name="transaction"></param>
        void AddTransaction(Transaction transaction);

        /// <summary>
        /// Get all pending transactions. Once transactions are returned the pool is cleared.
        /// </summary>
        /// <returns>All pending transactions.</returns>
        IEnumerable<Transaction> GetTransactions();
    }
}
