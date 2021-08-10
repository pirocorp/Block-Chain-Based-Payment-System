namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts
{
    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;

    public class AccountListingAdminModel : IMapFrom<Account>, IHaveCustomMappings
    {
        public string Address { get; set; }

        public double Balance { get; set; }

        public double BlockedBalance { get; set; }

        public bool HasKey { get; set; }

        public string User { get; set; }

        [IgnoreMap]
        public string FriendlyAddress => AddressHelpers.FriendlyAddress(this.Address);

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<Account, AccountListingAdminModel>()
                .ForMember(d => d.User, opt
                    => opt.MapFrom(s => s.User.UserName))
                .ForMember(d => d.HasKey, opt 
                    => opt.MapFrom(s => s.AccountKey.Address != null));
        }
    }
}
