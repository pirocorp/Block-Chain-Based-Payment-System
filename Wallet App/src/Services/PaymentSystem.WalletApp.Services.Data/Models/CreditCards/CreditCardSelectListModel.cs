namespace PaymentSystem.WalletApp.Services.Data.Models.CreditCards
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class CreditCardSelectListModel : IMapFrom<CreditCard>
    {
        public string Id { get; set; }

        public string LastFourDigits { get; set; }
    }
}
