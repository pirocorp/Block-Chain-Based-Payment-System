namespace PaymentSystem.Common.Utilities
{
    using System;
    using System.Numerics;
    using System.Security.Cryptography;

    using EllipticCurve;

    public class AccountKeys
    {
        public AccountKeys()
        {
            this.PrivateKey = new PrivateKey();
        }

        public AccountKeys(string secret)
        {
            this.PrivateKey = new PrivateKey(GlobalConstants.EllipticCurveParameter, secret.HexToBigInteger());
        }

        public BigInteger Secret => this.PrivateKey.secret;

        public PrivateKey PrivateKey { get; }

        public PublicKey PublicKey => this.PrivateKey.publicKey();

        public string Address => Convert.ToBase64String(SHA256.Create().ComputeHash(this.PublicKey.toString()));
    }
}
