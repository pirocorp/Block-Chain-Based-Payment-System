namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using PaymentSystem.WalletApp.Services.Data.Models;

    public interface IUserService
    {
        Task<T> GetUser<T>(string id);

        Task<IEnumerable<T>> GetBankAccounts<T>(string id);

        Task<IEnumerable<T>> GetCreditCards<T>(string id);

        Task<IEnumerable<SelectListItem>> GetPaymentMethods(string userId, PaymentMethod paymentMethod);

        Task<IEnumerable<SelectListItem>> GetCoinAccounts(string userId);

        Task<IEnumerable<T>> GetLatestRegisteredUsers<T>();

        Task<bool> SendCoins(string senderAddress, string recipientAddress, double amount, string secret, string userId);
    }
}
