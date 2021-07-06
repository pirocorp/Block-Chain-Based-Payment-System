namespace PaymentSystem.Common.Utilities
{
    using EllipticCurve;

    public static class AccountHelpers
    {
        public static AccountKeys CreateAccount() => new ();

        public static AccountKeys RestoreAccount(string secret) => new (secret);
    }
}
