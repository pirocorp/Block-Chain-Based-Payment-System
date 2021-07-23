namespace PaymentSystem.WalletApp.Web.ViewModels.Financials.Profile
{
    using System.Collections.Generic;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ProfileFinancialViewModel : IMapFrom<ApplicationUser>
    {
        public ProfileFinancialViewModel()
        {
            this.CreditCards = new List<ProfileCreditCardModel>();
            this.BankAccounts = new List<ProfileBankAccountModel>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicture { get; set; }

        public string ProfilePictureAddress { get; set; }

        public double TotalBalance { get; set; }

        public IEnumerable<ProfileCreditCardModel> CreditCards { get; set; }

        public IEnumerable<ProfileBankAccountModel> BankAccounts { get; set; }
    }
}
