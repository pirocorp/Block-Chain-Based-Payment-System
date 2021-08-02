namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.AccountsKeys;

    public interface IAccountsKeyService
    {
        Task StoreKeys(StoreAccountKeyServiceModel model, string userId);

        Task<KeyData> GetKeyData(string address, string userId);
    }
}
