namespace PaymentSystem.WalletApp.Web.ViewModels.Users.Profile
{
    using System;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout;

    public class ProfileUserViewModel : ProfileLayoutUserModel
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
    }
}
