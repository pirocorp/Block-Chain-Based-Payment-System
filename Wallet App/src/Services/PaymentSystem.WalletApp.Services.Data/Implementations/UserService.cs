namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data;

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<T> GetUser<T>(string id)
            => await this.dbContext.Users
                .Where(u => u.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
    }
}
