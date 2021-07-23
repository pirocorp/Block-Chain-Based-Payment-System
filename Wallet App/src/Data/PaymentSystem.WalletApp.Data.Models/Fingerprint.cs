namespace PaymentSystem.WalletApp.Data.Models
{
    using System;

    using PaymentSystem.WalletApp.Data.Common.Models;

    public class Fingerprint : BaseModel<string>
    {
        public Fingerprint()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Stamp { get; set; }
    }
}
