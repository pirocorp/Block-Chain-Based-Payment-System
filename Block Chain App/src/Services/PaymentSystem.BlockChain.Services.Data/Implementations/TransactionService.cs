namespace PaymentSystem.BlockChain.Services.Data.Implementations
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.BlockChain.Data;
    using PaymentSystem.Common.Data.Models;

    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext dbContext;

        public TransactionService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> Exists(string hash)
            => await this.dbContext.Transactions.AnyAsync(t => t.Hash == hash);

        public async Task<Transaction> Get(string hash)
            => await this.dbContext.Transactions.FirstOrDefaultAsync(t => t.Hash == hash);
    }
}