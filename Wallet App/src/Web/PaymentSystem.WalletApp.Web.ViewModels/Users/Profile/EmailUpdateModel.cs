namespace PaymentSystem.WalletApp.Web.ViewModels.Users.Profile
{
    using System.ComponentModel.DataAnnotations;

    public class EmailUpdateModel
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
