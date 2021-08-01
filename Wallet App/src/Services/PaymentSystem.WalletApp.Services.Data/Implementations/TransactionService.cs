namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data;

    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext dbContext;

        public TransactionService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<T> GetTransaction<T>(string hash)
            => await this.dbContext.Transactions
                .Where(t => t.Hash == hash)
                .To<T>()
                .FirstOrDefaultAsync();
    }
}
