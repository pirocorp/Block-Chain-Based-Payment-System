namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts
{
    using System;

    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class AccountDetailsModel : IMapFrom<Account>, IHaveCustomMappings
    {
        public string UserId { get; set; }

        public string UserUserName { get; set; }

        public string UserEmail { get; set; }

        public string UserPhoneNumber { get; set; }

        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string UserAddressStreet { get; set; }

        public string UserAddressCity { get; set; }

        public string UserAddressStateProvince { get; set; }

        public string UserAddressZip { get; set; }

        public string UserAddressCountry { get; set; }

        public bool UserIsDeleted { get; set; }

        public double Balance { get; set; }

        public double BlockedBalance { get; set; }

        public string AccountAddress { get; set; }

        public bool HasAccountKey { get; set; }

        public double TotalInflow { get; set; }

        public double TotalOutflow { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<Account, AccountDetailsModel>()
                .ForMember(d => d.AccountAddress, opt
                    => opt.MapFrom(s => s.Address))
                .ForMember(d => d.HasAccountKey, opt
                    => opt.MapFrom(s => s.AccountKey.Address != null));
        }
    }
}
