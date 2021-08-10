namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.Common.Transactions;
    using PaymentSystem.Common.Utilities;
    using PaymentSystem.WalletApp.Data;

    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IBlockChainGrpcService blockChainGrpcService;

        public TransactionService(
            ApplicationDbContext dbContext,
            IBlockChainGrpcService blockChainGrpcService)
        {
            this.dbContext = dbContext;
            this.blockChainGrpcService = blockChainGrpcService;
        }

        public async Task<T> GetTransaction<T>(string hash)
            => await this.dbContext.Transactions
                .Where(t => t.Hash == hash)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task<(int Total, IEnumerable<T> Transactions)> GetTransactionsForAddress<T>(string address, int page, int pageSize)
        {
            var query = this.dbContext.Transactions
                .Where(t => t.Sender == address || t.Recipient == address);

            var total = await query.CountAsync();

            var transactions = await query
                .OrderByDescending(t => t.TimeStamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .To<T>()
                .ToListAsync();

            return (total, transactions);
        }

        public async Task<(TransactionStatus Status, string Hash)> CreateTransaction(
            string senderAddress,
            string recipientAddress,
            double amount,
            double fee,
            string secret,
            string publicKey)
        {
            var sendRequest = CreateTransactionRequest(
                senderAddress,
                recipientAddress,
                amount,
                fee,
                secret,
                publicKey);

            var response = await this.blockChainGrpcService.AddTransactionToPool(sendRequest);

            var status = Enum.Parse<TransactionStatus>(response.Result, true);
            var hash = sendRequest.TransactionId;

            return (status, hash);
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
    }
}
