namespace PaymentSystem.WalletApp.Web.ViewModels.Financials.Profile
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ProfileBankAccountModel : IMapFrom<BankAccount>
    {
        public string Id { get; set; }

        public bool IsApproved { get; set; }

        public string BankName { get; set; }

        public string IBAN { get; set; }
    }
}
