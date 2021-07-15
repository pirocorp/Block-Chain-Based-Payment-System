namespace PaymentSystem.WalletApp.Web.ViewModels.Users.Profile
{
    using System.ComponentModel.DataAnnotations;

    public class PhoneUpdateModel
    {
        [Phone]
        public string Phone { get; set; }
    }
}
