namespace PaymentSystem.WalletApp.Data.Common
{
    public static class DataConstants
    {
        public static class Account
        {
            public const int NameLength = 100;
        }

        public static class Address
        {
            public const int StreetLength = 150;

            public const int CityLength = 100;

            public const int StateProvinceLength = 100;

            public const int ZipLength = 50;

            public const int CountryLength = 100;
        }

        public static class ApplicationUser
        {
            public const int FirstNameLength = 100;

            public const int LastNameLength = 100;

            public const int ProfilePictureLength = 100;
        }

        public static class BankAccount
        {
            public const int CountryLength = 100;

            public const int BankNameLength = 150;

            public const int AccountHolderNameLength = 300;

            public const int IBANLength = 34;

            public const int IBANMinLength = 5;

            public const int SwiftLength = 11;

            public const int SwiftMinLength = 8;
        }

        public static class Beneficiary
        {
            public const int NameLength = 300;
        }

        public static class CreditCard
        {
            public const int LastFourDigitsLength = 4;

            public const int ExpiryDateLength = 5;

            public const int SecurityStampLength = 128;
        }

        public static class Testimonial
        {
            public const int CommentLength = 5000;

            public const int NameLength = 300;

            public const int UseCaseLength = 100;
        }
    }
}
