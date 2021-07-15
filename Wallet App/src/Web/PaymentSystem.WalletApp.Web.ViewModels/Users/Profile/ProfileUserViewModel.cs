namespace PaymentSystem.WalletApp.Web.ViewModels.Users.Profile
{
    using System;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ProfileUserViewModel : IMapFrom<ApplicationUser>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicture { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Address Address { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public double TotalBalance { get; set; }

        public string ProfilePictureAddress { get; set; }

        public bool HaveAddress =>
               this.Address.Street != null
            || this.Address.City != null
            || this.Address.StateProvince != null
            || this.Address.Zip != null
            || this.Address.Country != null;
    }
}
