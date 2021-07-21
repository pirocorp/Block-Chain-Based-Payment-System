namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Services.Data.Models.CreditCards;

    public interface ICreditCardService
    {
        Task<bool> Exists(string cardId);

        Task<bool> UserOwnsCard(string cardId, string userId);

        Task AddCreditCard(AddCreditCardServiceModel model, string userId, byte[] key);

        Task<IEnumerable<T>> GetCreditCards<T>(string userId);

        Task<CreditCardDetailsServiceModel> GetCardInformation(string cardId, byte[] key);

        Task UpdateCardInformation(byte[] key, EditCreditCardServiceModel model);

        Task DeleteCreditCard(string cardId);
    }
}
