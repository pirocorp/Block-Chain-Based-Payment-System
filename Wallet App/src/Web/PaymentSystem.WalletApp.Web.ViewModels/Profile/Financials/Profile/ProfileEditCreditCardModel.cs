namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Financials.Profile
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.CreditCards;

    public class ProfileEditCreditCardModel : IMapFrom<CreditCardDetailsServiceModel>
    {
        public string Id { get; set; }

        public string LastFourDigits { get; set; }

        public string ExpiryDate { get; set; }

        public CardType CardType { get; set; }

        public string CVV { get; set; }

        public string CardHolderName { get; set; }
    }
}
