namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Accounts.Profile
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.ViewModels.Profile.ProfileLayout;

    public class ProfileAccountsViewModel : ProfileLayoutUserModel, IHaveCustomMappings
    {
        public ProfileAccountsViewModel()
        {
            this.Accounts = new List<ProfileAccountModel>();
        }

        public IEnumerable<ProfileAccountModel> Accounts { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ApplicationUser, ProfileAccountsViewModel>()
                .ForMember(p => p.TotalBalance, opt
                    => opt.MapFrom(a => a.Accounts.Sum(a => a.Balance)));
        }
    }
}
