namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Users.Profile
{
    using System.ComponentModel.DataAnnotations;

    public class EmailUpdateModel
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
