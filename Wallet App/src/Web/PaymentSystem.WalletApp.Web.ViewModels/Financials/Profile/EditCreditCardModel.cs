namespace PaymentSystem.WalletApp.Web.ViewModels.Financials.Profile
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Services.Data.Models.CreditCards;
    using PaymentSystem.WalletApp.Web.Infrastructure.ValidationAttributes;

    using static PaymentSystem.WalletApp.Web.Infrastructure.WebConstants;

    public class EditCreditCardModel : IMapTo<EditCreditCardServiceModel>
    {
        [Required]
        public string Id { get; set; }

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
