namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using PaymentSystem.Common.Data.Models;
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

        public async Task<(int Total, IEnumerable<T> Transactions)> GetBlockTransactions<T>(string blockHash, int page, int pageSize)
        {
            var query = this.dbContext.Transactions
                .Where(t => t.BlockHash == blockHash)
                .OrderBy(t => t.TimeStamp);

            return await GetTransactionsByPages<T>(query, page, pageSize);
        }

        public async Task<(int Total, IEnumerable<T> Transactions)> GetAccountTransactions<T>(string address, int page, int pageSize)
        {
            var query = this.dbContext.Transactions
                .Where(t => t.Recipient == address || t.Sender == address)
                .OrderByDescending(t => t.TimeStamp);

            return await GetTransactionsByPages<T>(query, page, pageSize);
        }

        public async Task<(double TotalInflow, double TotalOutflow)> GetAccountMoneyFlow(string address)
        {
            var inflow = await this.dbContext.Transactions
                .Where(t => t.Recipient == address)
                .SumAsync(t => t.Amount);

            var outflow = await this.dbContext.Transactions
                .Where(t => t.Sender == address)
                .SumAsync(t => t.Amount + t.Fee);

            return (inflow, outflow);
        }

        private static async Task<(int Total, IEnumerable<T>)> GetTransactionsByPages<T>(IQueryable<Transaction> query, int page, int pageSize)
        {
            var total = await query.CountAsync();
            var transactions = await query
                .To<T>()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, transactions);
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
