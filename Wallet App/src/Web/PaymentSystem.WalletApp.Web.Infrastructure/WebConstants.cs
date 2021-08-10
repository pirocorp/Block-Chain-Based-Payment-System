namespace PaymentSystem.WalletApp.Web.Infrastructure
{
    public static class WebConstants
    {
        public const int CardHolderNameLength = 300;

        public const string DepositDescription = "Deposit to coin account.";

        public const string WithdrawDescription = "Withdraw to {0} bank account.";

        public const string ReceiveDescription = "Receive {0} amount from address {1}";

        public const string SendDescription = "Send {0} amount to address {1}";

        public const int DefaultActivitiesResultPageSize = 10;

        public const int DefaultTransactionsResultPageSize = 10;

        public static class DepositConfirmInputModel
        {
            public const int PaymentMethodMinLength = 30;

            public const int PaymentMethodMaxLength = 40;

            public const int AccountMinLength = 30;

            public const int AccountMaxLength = 128;
        }

        public static class WithdrawInputModel
        {
            public const string AmountErrorMessage = "Amount must be positive.";

            public const string BankAccountErrorMessage = "Invalid Bank Account";

            public const string CoinAccountErrorMessage = "Invalid Coin Account";

            public const string InsufficientFundsErrorMessage = "Insufficient Funds";
        }

        public static class SendCoin
        {
            public const string AmountErrorMessage = "Amount must be positive.";

            public const string CoinAccountErrorMessage = "Invalid Coin Account";

            public const string InsufficientFundsErrorMessage = "Insufficient Funds";
        }
    }
}
