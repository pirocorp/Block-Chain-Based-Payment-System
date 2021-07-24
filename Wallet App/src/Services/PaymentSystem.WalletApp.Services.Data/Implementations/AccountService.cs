namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;

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

        public async Task<bool> UserOwnsAccount(string userId, string address)
            => await this.dbContext.Accounts
                .AnyAsync(a => a.Address == address && a.UserId == userId);

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
    }
}
