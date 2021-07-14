namespace PaymentSystem.WalletApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    [Owned]
    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string StateProvince { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }
    }
}
