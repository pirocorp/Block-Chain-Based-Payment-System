namespace PaymentSystem.BlockChain.Services.Data.Implementations
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.BlockChain.Data;
    using PaymentSystem.BlockChain.Data.Models;
    using PaymentSystem.Common;
    using PaymentSystem.Common.Utilities;
    using PaymentSystem.BlockChain.Services.Data.Models;
    
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext context;

        public AccountService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> Exists(string address)
            => await this.context.Accounts.AnyAsync(x => x.Address == address);

        public async Task<AccountServiceModel> Create()
        {
            var accountKeys = AccountHelpers.CreateAccount();

            var account = new Account()
            {
                Address = accountKeys.Address,
                PublicKey = accountKeys.PublicKey.PublicKeyToString(),
                Balance = GlobalConstants.WelcomeBonus,
            };

            await this.context.AddAsync(account);
            await this.context.SaveChangesAsync();

            var model = new AccountServiceModel()
            {
                Address = account.Address,
                PublicKey = account.PublicKey,
                Balance = account.Balance,
                Secret = accountKeys.Secret.BigIntegerToHex(),
            };

            return model;
        }

        /// <summary>
        /// Most block chain implementations sends money to address that are not in the system.
        /// There are number of such bitcoins in 'not valid' addresses.
        /// </summary>
        public async Task<bool> TryDeposit(string address, double amount)
        {
            var account = await this.GetAccount(address);

            if (account is null)
            {
                return false;
            }

            account.Balance += amount;
            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TryWithdraw(string address, double amount)
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
