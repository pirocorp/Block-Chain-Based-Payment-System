namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Financials.Profile
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.ViewModels.Profile.ProfileLayout;

    public class ProfileFinancialViewModel : ProfileLayoutUserModel, IHaveCustomMappings
    {
        public ProfileFinancialViewModel()
        {
            this.CreditCards = new List<ProfileCreditCardModel>();
            this.BankAccounts = new List<ProfileBankAccountModel>();
        }

        public string Id { get; set; }

        public IEnumerable<ProfileCreditCardModel> CreditCards { get; set; }

        public IEnumerable<ProfileBankAccountModel> BankAccounts { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ApplicationUser, ProfileFinancialViewModel>()
                .ForMember(p => p.TotalBalance, opt
                    => opt.MapFrom(a => a.Accounts.Sum(a => a.Balance)));
        }
    }
}
