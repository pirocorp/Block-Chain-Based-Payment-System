namespace PaymentSystem.WalletApp.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;

    using static Data.Common.DataConstants.Address;

    [Owned]
    public class Address
    {
        [StringLength(StreetLength)]
        public string Street { get; set; }

        [StringLength(CityLength)]
        public string City { get; set; }

        [StringLength(StateProvinceLength)]
        public string StateProvince { get; set; }

        [StringLength(ZipLength)]
        public string Zip { get; set; }

        [StringLength(CountryLength)]
        public string Country { get; set; }
    }
}
