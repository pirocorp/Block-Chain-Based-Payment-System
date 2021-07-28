namespace PaymentSystem.WalletApp.Web.Infrastructure
{
    public static class WebConstants
    {
        public const int CardHolderNameLength = 300;

        public const string DepositDescription = "Deposit to coin account.";

        public static class DepositConfirmInputModel
        {
            public const int PaymentMethodMinLength = 30;

            public const int PaymentMethodMaxLength = 40;

            public const int AccountMinLength = 30;

            public const int AccountMaxLength = 128;
        }
    }
}
