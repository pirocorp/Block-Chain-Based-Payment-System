namespace PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ProfileLayoutUserModel : IMapFrom<ApplicationUser>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicture { get; set; }

        public double TotalBalance { get; set; }

        public string ProfilePictureAddress { get; set; }
    }
}
