namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    using PaymentSystem.Common.Transactions;

    public interface ITransactionService
    {
        Task<T> GetTransaction<T>(string hash);

        Task<(TransactionStatus Status, string Hash)> CreateTransaction(string senderAddress, string recipientAddress, double amount, double fee, string secret, string publicKey);
    }
}
