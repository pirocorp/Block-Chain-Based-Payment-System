namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    public interface ITransactionService
    {
        Task<T> GetTransaction<T>(string hash);
    }
}
