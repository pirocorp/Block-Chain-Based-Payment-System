namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Financials.Profile
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ProfileCreditCardModel : IMapFrom<CreditCard>
    {
        public string Id { get; set; }

        public string LastFourDigits { get; set; }

        public string ExpiryDate { get; set; }

        public CardType CardType { get; set; }
    }
}
