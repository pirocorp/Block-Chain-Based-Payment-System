namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Users.Profile
{
    using System;
    using System.Linq;

    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.ViewModels.Profile.ProfileLayout;

    public class ProfileUserViewModel : ProfileLayoutUserModel, IHaveCustomMappings
    {
        public DateTime DateOfBirth { get; set; }

        public Address Address { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool HaveAddress =>
               this.Address.Street != null
            || this.Address.City != null
            || this.Address.StateProvince != null
            || this.Address.Zip != null
            || this.Address.Country != null;

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ApplicationUser, ProfileUserViewModel>()
                .ForMember(d => d.TotalBalance, opt
                    => opt.MapFrom(s => s.Accounts.Sum(a => a.Balance)));
        }
    }
}
