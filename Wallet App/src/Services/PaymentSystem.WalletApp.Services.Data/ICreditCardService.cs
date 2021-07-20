namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Services.Data.Models;

    public interface ICreditCardService
    {
        Task AddCreditCard(AddCreditCardServiceModel model, string userId, byte[] key);

        Task<IEnumerable<T>> GetCreditCards<T>(string userId);
    }
}
