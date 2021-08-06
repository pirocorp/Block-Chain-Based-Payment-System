namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Financials.Profile
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ProfileDeleteBankAccountModel : IMapFrom<BankAccount>
    {
        public string Id { get; set; }

        public string AccountHolderName { get; set; }

        public string BankName { get; set; }

        public string Country { get; set; }

        public string IBAN { get; set; }

        public bool IsApproved { get; set; }
    }
}
