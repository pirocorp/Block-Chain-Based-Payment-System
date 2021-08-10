namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.Common.Transactions;

    public interface ITransactionService
    {
        Task<T> GetTransaction<T>(string hash);

        Task<(int Total, IEnumerable<T> Transactions)> GetTransactionsForAddress<T>(string address, int page, int pageSize);

        Task<(TransactionStatus Status, string Hash)> CreateTransaction(string senderAddress, string recipientAddress, double amount, double fee, string secret, string publicKey);
    }
}
