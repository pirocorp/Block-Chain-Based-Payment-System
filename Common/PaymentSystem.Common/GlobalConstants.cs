namespace PaymentSystem.Common
{
    public static class GlobalConstants
    {
        public const string EllipticCurveParameter = "secp256k1";

        public const int BlockChainBatchSize = 100;

        public const double WelcomeBonus = 0;

        public const string PushNotificationUrl = "/notification";

        public const int MaxBlockPageSize = 5;

        public const string ChanelAddress = "https://localhost:8081";

        public const double DefaultDepositFee = 0;

        public const double DefaultWithdrawFee = 0;

        public static class Block
        {
            public const int Version = 1;

            public const int DefaultDifficulty = 1;
        }
    }
}
