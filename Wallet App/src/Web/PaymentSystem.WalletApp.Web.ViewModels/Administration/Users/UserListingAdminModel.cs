namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Users
{
    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class UserListingAdminModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public bool HasPhoneAdded { get; set; }

        public bool HasEmailAdded { get; set; }

        public int AccountsCount { get; set; }

        public int AccountKeysCount { get; set; }

        public int BankAccountsCount { get; set; }

        public int CreditCardsCount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ApplicationUser, UserListingAdminModel>()
                .ForMember(d => d.HasPhoneAdded, opt
                    => opt.MapFrom(s => !string.IsNullOrWhiteSpace(s.PhoneNumber)))
                .ForMember(d => d.HasEmailAdded, opt
                    => opt.MapFrom(s => !string.IsNullOrWhiteSpace(s.Email)));
        }
    }
}
