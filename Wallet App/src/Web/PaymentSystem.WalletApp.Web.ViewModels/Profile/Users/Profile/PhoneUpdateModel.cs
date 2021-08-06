namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Users.Profile
{
    using System.ComponentModel.DataAnnotations;

    public class PhoneUpdateModel
    {
        [Phone]
        public string Phone { get; set; }
    }
}
