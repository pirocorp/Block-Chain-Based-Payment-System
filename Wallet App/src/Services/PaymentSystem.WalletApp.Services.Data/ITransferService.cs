namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Services.Data.Models.Transactions;

    public interface ITransferService
    {
        Task<bool> DepositToAccount(string userId, CreateTransactionServiceModel model);
    }
}
