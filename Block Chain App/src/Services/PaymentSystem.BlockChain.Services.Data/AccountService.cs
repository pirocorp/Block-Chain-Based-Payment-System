namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.BlockChain.Data;
    using PaymentSystem.BlockChain.Data.Models;
    using PaymentSystem.Common;

    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext context;

        public AccountService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> Exists(string address)
            => await this.context.Accounts.AnyAsync(x => x.Address == address);

        public async Task Create(string address)
        {
            var account = new Account()
            {
                Address = address,
                Balance = GlobalConstants.WelcomeBonus,
            };

            await this.context.AddAsync(account);
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Most block chain implementations sends money to address that are not in the system.
        /// There are number of such bitcoins in 'not valid' addresses.
        /// </summary>
        public async Task Deposit(string address, double amount)
        {
            var account = await this.GetAccount(address);

            if (account is null)
            {
                await this.Create(address);
                account = await this.GetAccount(address);
            }

            account.Balance += amount;
            await this.context.SaveChangesAsync();
        }

        public async Task<bool> Withdraw(string address, double amount)
        {
            var account = await this.GetAccount(address);

            if (account is null)
            {
                return false;
            }

            if (account.Balance < amount)
            {
                return false;
            }

            account.Balance -= amount;
            await this.context.SaveChangesAsync();

            return true;
        }

        private async Task<Account> GetAccount(string address)
            => await this.context.Accounts.FirstOrDefaultAsync(x => x.Address == address);
    }
}
