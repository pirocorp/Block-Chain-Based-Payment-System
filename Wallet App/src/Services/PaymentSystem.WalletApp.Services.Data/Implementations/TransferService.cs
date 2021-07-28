namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Options;

    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.Common.Transactions;
    using PaymentSystem.Common.Utilities;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.Activities;
    using PaymentSystem.WalletApp.Services.Data.Models.Transactions;
    using PaymentSystem.WalletApp.Web.Infrastructure;
    using PaymentSystem.WalletApp.Web.Infrastructure.Options;

    public class TransferService : ITransferService
    {
        private readonly IOptions<WalletProviderOptions> walletProviderOptions;
        private readonly IBlockChainGrpcService blockChainGrpcService;
        private readonly IActivityService activityService;

        public TransferService(
            IOptions<WalletProviderOptions> walletProviderOptions,
            IBlockChainGrpcService blockChainGrpcService,
            IActivityService activityService)
        {
            this.walletProviderOptions = walletProviderOptions;
            this.blockChainGrpcService = blockChainGrpcService;
            this.activityService = activityService;
        }

        public async Task<bool> DepositToAccount(string userId, CreateTransactionServiceModel model)
        {
            var transactionInput = new TransactionInput()
            {
                SenderAddress = this.walletProviderOptions.Value.Address,
                TimeStamp = DateTime.UtcNow.Ticks,
            };

            var transactionOutput = new TransactionOutput()
            {
                RecipientAddress = model.RecipientAddress,
                Amount = model.Amount,
                Fee = 0,
            };

            var transactionHash = BlockChainHashing
                .GenerateTransactionHash(
                    transactionInput.TimeStamp,
                    transactionInput.SenderAddress,
                    transactionOutput.RecipientAddress,
                    transactionOutput.Amount,
                    transactionOutput.Fee);

            var signature = BlockChainHashing
                .CreateSignature(transactionHash, this.walletProviderOptions.Value.Secret);

            transactionInput.Signature = signature;

            var sendRequest = new SendRequest()
            {
                TransactionId = transactionHash,
                PublicKey = this.walletProviderOptions.Value.PublicKey,
                TransactionInput = transactionInput,
                TransactionOutput = transactionOutput,
            };

            var response = await this.blockChainGrpcService.AddTransactionToPool(sendRequest);
            var success = response.Result != TransactionStatus.Canceled.ToString();

            var activity = new ActivityServiceModel
            {
                Amount = model.Amount,
                CounterpartyAddress = $"XXXX - {model.RecipientAddress[^12..]}",
                Description = WebConstants.DepositDescription,
                UserId = userId,
                Status = success ? ActivityStatus.Pending : ActivityStatus.Canceled,
            };

            await this.activityService.AddActivity(activity);
            return success;
        }
    }
}
