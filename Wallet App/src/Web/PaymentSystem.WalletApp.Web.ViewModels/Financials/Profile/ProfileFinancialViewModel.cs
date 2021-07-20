namespace PaymentSystem.WalletApp.Web.ViewModels.Financials.Profile
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ProfileFinancialViewModel : IMapFrom<ApplicationUser>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicture { get; set; }

        public string ProfilePictureAddress { get; set; }

        public double TotalBalance { get; set; }
    }
}
