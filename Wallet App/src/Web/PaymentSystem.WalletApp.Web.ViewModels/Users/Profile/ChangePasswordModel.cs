namespace PaymentSystem.WalletApp.Web.ViewModels.Users.Profile
{
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}
