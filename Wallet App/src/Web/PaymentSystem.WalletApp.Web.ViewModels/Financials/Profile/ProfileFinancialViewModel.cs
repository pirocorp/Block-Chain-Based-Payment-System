namespace PaymentSystem.WalletApp.Web.ViewModels.Financials.Profile
{
    using System.Collections.Generic;

    using PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout;

    public class ProfileFinancialViewModel : ProfileLayoutUserModel
    {
        public ProfileFinancialViewModel()
        {
            this.CreditCards = new List<ProfileCreditCardModel>();
            this.BankAccounts = new List<ProfileBankAccountModel>();
        }

        public string Id { get; set; }

        public IEnumerable<ProfileCreditCardModel> CreditCards { get; set; }

        public IEnumerable<ProfileBankAccountModel> BankAccounts { get; set; }
    }
}
