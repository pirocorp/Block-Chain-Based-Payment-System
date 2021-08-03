namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    using PaymentSystem.Common;
    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.Common.Transactions;
    using PaymentSystem.Common.Utilities;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.Activities;
    using PaymentSystem.WalletApp.Services.Data.Models.Transactions;
    using PaymentSystem.WalletApp.Web.Infrastructure;
    using PaymentSystem.WalletApp.Web.Infrastructure.Options;

    public class TransferService : ITransferService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAccountService accountService;
        private readonly IAccountsKeyService accountsKeyService;
        private readonly IActivityService activityService;
        private readonly IBlockChainGrpcService blockChainGrpcService;
        private readonly IOptions<WalletProviderOptions> walletProviderOptions;
        private readonly ITransactionService transactionService;

        public TransferService(
            ApplicationDbContext dbContext,
            IAccountService accountService,
            IAccountsKeyService accountsKeyService,
            IActivityService activityService,
            IBlockChainGrpcService blockChainGrpcService,
            IOptions<WalletProviderOptions> walletProviderOptions,
            ITransactionService transactionService)
        {
            this.dbContext = dbContext;
            this.accountService = accountService;
            this.accountsKeyService = accountsKeyService;
            this.activityService = activityService;
            this.blockChainGrpcService = blockChainGrpcService;
            this.walletProviderOptions = walletProviderOptions;
            this.transactionService = transactionService;
        }

        public async Task<bool> DepositToAccount(string userId, DepositServiceModel model)
        {
            var (transactionStatus, transactionHash) = await this.transactionService.CreateTransaction(
                    this.walletProviderOptions.Value.Address,
                    model.RecipientAddress,
                    model.Amount,
                    model.Fee,
                    this.walletProviderOptions.Value.Secret,
                    this.walletProviderOptions.Value.PublicKey);

            var success = TransactionIsSuccessful(transactionStatus);

            var activity = new ActivityServiceModel
            {
                Amount = model.Amount,
                CounterpartyAddress = $"XXXX - {model.RecipientAddress[^12..]}",
                Description = WebConstants.DepositDescription,
                UserId = userId,
                Status = success ? ActivityStatus.Pending : ActivityStatus.Canceled,
                TransactionHash = transactionHash,
            };

            await this.activityService.AddActivity(activity);
            return success;
        }

        public async Task<bool> WithdrawFromAccount(string userId, WithdrawServiceModel model)
        {
            var keyData = await this.accountsKeyService.GetKeyData(model.CoinAccount, userId);
            var publicKey = await this.accountService.GetPublicKey(model.CoinAccount);

            var (transactionStatus, transactionHash) = await this.transactionService.CreateTransaction(
                model.CoinAccount,
                this.walletProviderOptions.Value.Address,
                model.Amount,
                GlobalConstants.DefaultWithdrawFee,
                keyData.Secret,
                publicKey);

            var bankAccount = await this.dbContext.BankAccounts.FirstOrDefaultAsync(b => b.Id == model.BankAccount);
            var description = string.Format(WebConstants.WithdrawDescription, $"{bankAccount.BankName} - {bankAccount.IBAN[^12..]}");

            var success = TransactionIsSuccessful(transactionStatus);

            if (success)
            {
                await this.accountService.BlockFunds(model.CoinAccount, model.Amount);
            }

            var activity = new ActivityServiceModel
            {
                Amount = model.Amount,
                CounterpartyAddress = $"XXXX - {this.walletProviderOptions.Value.Address[^12..]}",
                Description = description,
                UserId = userId,
                Status = success ? ActivityStatus.Pending : ActivityStatus.Canceled,
                TransactionHash = transactionHash,
                BlockedAmount = model.Amount,
            };

            await this.activityService.AddActivity(activity);

            return success;
        }

        private static bool TransactionIsSuccessful(TransactionStatus transactionStatus)
            => transactionStatus == TransactionStatus.Pending;
    }
}
