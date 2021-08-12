namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Users
{
    using System.Collections.Generic;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts;

    public class UserDetailsAdminModel : IMapFrom<ApplicationUser>
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AddressStreet { get; set; }

        public string AddressCity { get; set; }

        public string AddressStateProvince { get; set; }

        public string AddressZip { get; set; }

        public string AddressCountry { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<AccountListingAdminModel> Accounts { get; set; }
    }
}
