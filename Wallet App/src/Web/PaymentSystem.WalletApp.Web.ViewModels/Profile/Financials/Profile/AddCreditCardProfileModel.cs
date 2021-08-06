namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Financials.Profile
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.CreditCards;
    using PaymentSystem.WalletApp.Web.Infrastructure.ValidationAttributes;

    using static Infrastructure.WebConstants;

    public class AddCreditCardProfileModel : IMapTo<AddCreditCardServiceModel>
    {
        [Required]
        public bool IsCredit { get; set; }

        public CardType CardType { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [CreditCardExpiryDate]
        public string ExpiryDate { get; set; }

        [Required]
        [CVV]
        public string CVV { get; set; }

        [Required]
        [StringLength(CardHolderNameLength)]
        public string CardHolderName { get; set; }
    }
}
