namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Services.Data.Models.BankAccounts;

    public interface IBankAccountService
    {
        Task<bool> Exists(string id);

        Task<bool> UserOwnsAccount(string id, string userId);

        Task<T> GetAccountInformation<T>(string id);

        Task<IEnumerable<T>> GetAccounts<T>(string userId);

        Task AddAccount(AddBankAccountServiceModel model, string userId);

        Task DeleteAccount(string id);
    }
}
