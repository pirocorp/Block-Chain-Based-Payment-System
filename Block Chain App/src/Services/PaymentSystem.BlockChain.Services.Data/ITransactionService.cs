namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.Common.Data.Models;

    public interface ITransactionService
    {
        Task<bool> Exists(string hash);

        Task<Transaction> Get(string hash);

        Task AddRange(IEnumerable<Transaction> transactions);

        Task Add(Transaction transaction);
    }
}
