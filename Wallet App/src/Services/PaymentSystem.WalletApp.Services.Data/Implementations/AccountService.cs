namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.Accounts;

    using static Web.Infrastructure.WebConstants;

    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IBlockChainGrpcService blockChainGrpcService;

        public AccountService(
            ApplicationDbContext dbContext,
            IBlockChainGrpcService blockChainGrpcService)
        {
            this.dbContext = dbContext;
            this.blockChainGrpcService = blockChainGrpcService;
        }

        public async Task<bool> Exists(string address)
            => await this.dbContext.Accounts.AnyAsync(a => a.Address == address);

        public async Task<bool> UserOwnsAccount(string address, string userId)
            => await this.dbContext.Accounts
                .AnyAsync(a => a.Address == address && a.UserId == userId);

        public async Task<bool> HasSufficientFunds(string address, double amount)
            => await this.dbContext.Accounts
                .AnyAsync(a => a.Address == address && a.Balance >= amount);

        public async Task<string> GetPublicKey(string address)
            => (await this.dbContext.Accounts.FirstOrDefaultAsync(a => a.Address == address)).PublicKey;

        public async Task<string> GetUserId(string address)
            => (await this.dbContext.Accounts.FirstOrDefaultAsync(a => a.Address == address)).UserId;

        public async Task<T> GetAccount<T>(string address)
            => await this.dbContext.Accounts
                .Where(a => a.Address == address)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<T>> GetLatestAccounts<T>()
            => await this.dbContext.Accounts
                .OrderByDescending(a => a.CreatedOn)
                .To<T>()
                .Take(DefaultAccountsResultPageSize)
                .ToListAsync();

        public async Task<(int Total, IEnumerable<T> Accounts)> GetAccounts<T>(int page, int pageSize)
        {
            var total = await this.dbContext.Accounts.CountAsync();

            var accounts = await this.dbContext.Accounts
                .OrderBy(a => a.Address)
                .To<T>()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, accounts);
        }

        public async Task<IEnumerable<T>> GetUserAccounts<T>(string userId)
            => await this.dbContext.Accounts
                .Where(a => a.UserId == userId)
                .To<T>()
                .ToListAsync();

        public async Task<AccountCreationResponse> Create(string userId)
        {
            var blockchainAccount = await this.blockChainGrpcService.CreateAccount();

            var account = new Account()
            {
                Address = blockchainAccount.Address,
                Balance = blockchainAccount.Balance,
                PublicKey = blockchainAccount.PublicKey,
                UserId = userId,
            };

            await this.dbContext.AddAsync(account);
            await this.dbContext.SaveChangesAsync();

            return blockchainAccount;
        }

        public async Task BlockFunds(string address, double amount)
        {
            var account = await this.dbContext.Accounts.FindAsync(address);

            account.Balance -= amount;
            account.BlockedBalance += amount;
        }

        public async Task Deposit(string address, double amount)
        {
            var account = await this.dbContext.Accounts.FindAsync(address);

            account.Balance += amount;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task Withdraw(string address, double amount)
        {
            var account = await this.dbContext.Accounts.FindAsync(address);

            if (account.BlockedBalance > 0)
            {
                amount -= account.BlockedBalance;
                account.BlockedBalance = 0;
            }

            if (amount != 0)
            {
                account.Balance -= amount;
            }

            await this.dbContext.SaveChangesAsync();
        }

        public async Task EditAccount(EditAccountServiceModel model)
        {
            var account = await this.dbContext.Accounts.FindAsync(model.Address);
            account.Name = model.Name;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task Delete(string address)
        {
            await this.blockChainGrpcService.DeleteAccount(address);

            var account = await this.dbContext.Accounts.FindAsync(address);

            var accountKeys = await this.dbContext.AccountsKeys
                .Where(ak => ak.Address == address)
                .ToListAsync();

            this.dbContext.AccountsKeys.RemoveRange(accountKeys);
            this.dbContext.Accounts.Remove(account);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
