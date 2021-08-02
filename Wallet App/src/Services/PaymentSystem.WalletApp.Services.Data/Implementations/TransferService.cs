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

        public TransferService(
            ApplicationDbContext dbContext,
            IAccountService accountService,
            IAccountsKeyService accountsKeyService,
            IActivityService activityService,
            IBlockChainGrpcService blockChainGrpcService,
            IOptions<WalletProviderOptions> walletProviderOptions)
        {
            this.dbContext = dbContext;
            this.accountService = accountService;
            this.accountsKeyService = accountsKeyService;
            this.activityService = activityService;
            this.blockChainGrpcService = blockChainGrpcService;
            this.walletProviderOptions = walletProviderOptions;
        }

        public async Task<bool> DepositToAccount(string userId, DepositServiceModel model)
        {
            var sendRequest = CreateTransactionRequest(
                    this.walletProviderOptions.Value.Address,
                    model.RecipientAddress,
                    model.Amount,
                    model.Fee,
                    this.walletProviderOptions.Value.Secret,
                    this.walletProviderOptions.Value.PublicKey);

            var success = await this.SendTransactionRequest(sendRequest, userId, WebConstants.DepositDescription);

            return success;
        }

        public async Task<bool> WithdrawFromAccount(string userId, WithdrawServiceModel model)
        {
            var keyData = await this.accountsKeyService.GetKeyData(model.CoinAccount, userId);
            var publicKey = await this.accountService.GetPublicKey(model.CoinAccount);

            var sendRequest = CreateTransactionRequest(
                model.CoinAccount,
                this.walletProviderOptions.Value.Address,
                model.Amount,
                GlobalConstants.DefaultWithdrawFee,
                keyData.Secret,
                publicKey);

            var bankAccount = await this.dbContext.BankAccounts.FirstOrDefaultAsync(b => b.Id == model.BankAccount);
            var description = string.Format(WebConstants.WithdrawDescription, $"{bankAccount.BankName} - {bankAccount.IBAN[^12..]}");

            await this.accountService.BlockFunds(model.CoinAccount, model.Amount);

            var success = await this.SendTransactionRequest(sendRequest, userId, description);
            return success;
        }

        private static SendRequest CreateTransactionRequest(
            string senderAddress,
            string recipientAddress,
            double amount,
            double fee,
            string secret,
            string publicKey)
        {
            var transactionInput = new TransactionInput()
            {
                SenderAddress = senderAddress,
                TimeStamp = DateTime.UtcNow.Ticks,
            };

            var transactionOutput = new TransactionOutput()
            {
                RecipientAddress = recipientAddress,
                Amount = amount,
                Fee = fee,
            };

            var transactionHash = BlockChainHashing
                .GenerateTransactionHash(
                    transactionInput.TimeStamp,
                    transactionInput.SenderAddress,
                    transactionOutput.RecipientAddress,
                    transactionOutput.Amount,
                    transactionOutput.Fee);

            transactionInput.Signature = BlockChainHashing.CreateSignature(transactionHash, secret);

            var sendRequest = new SendRequest()
            {
                TransactionId = transactionHash,
                PublicKey = publicKey,
                TransactionInput = transactionInput,
                TransactionOutput = transactionOutput,
            };

            return sendRequest;
        }

        private async Task<bool> SendTransactionRequest(SendRequest sendRequest, string userId, string description)
        {
            var response = await this.blockChainGrpcService.AddTransactionToPool(sendRequest);
            var success = response.Result != TransactionStatus.Canceled.ToString();

            var activity = new ActivityServiceModel
            {
                Amount = sendRequest.TransactionOutput.Amount,
                CounterpartyAddress = $"XXXX - {sendRequest.TransactionOutput.RecipientAddress[^12..]}",
                Description = description,
                UserId = userId,
                Status = success ? ActivityStatus.Pending : ActivityStatus.Canceled,
                TransactionHash = sendRequest.TransactionId,
            };

            await this.activityService.AddActivity(activity);
            return success;
        }
    }
}
