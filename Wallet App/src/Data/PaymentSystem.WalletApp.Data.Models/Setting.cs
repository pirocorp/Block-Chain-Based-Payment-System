namespace PaymentSystem.WalletApp.Data.Models
{
    using System;

    using PaymentSystem.WalletApp.Data.Common.Models;

    public class Setting : BaseModel<string>
    {
        public Setting()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
