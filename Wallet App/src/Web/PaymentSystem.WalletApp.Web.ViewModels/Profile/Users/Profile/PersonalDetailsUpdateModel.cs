namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Users.Profile
{
    using System.ComponentModel.DataAnnotations;

    public class PersonalDetailsUpdateModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string BirthDate { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string StateProvince { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string Country { get; set; }
    }
}
