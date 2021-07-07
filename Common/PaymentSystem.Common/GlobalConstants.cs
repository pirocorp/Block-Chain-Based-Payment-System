namespace PaymentSystem.Common
{
    public static class GlobalConstants
    {
        public const string EllipticCurveParameter = "secp256k1";

        public const int BlockChainBatchSize = 100;

        public const double WelcomeBonus = 500;

        public const string PushNotificationUrl = "/notification";

        public static class Block
        {
            public const int Version = 1;

            public const int DefaultDifficulty = 1;
        }
    }
}
