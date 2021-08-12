namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Transactions
{
    using System;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Mapping;

    public class TransactionListingAdminModel : IMapFrom<Transaction>
    {
        public string Hash { get; set; }

        public long TimeStamp { get; set; }

        public string Sender { get; set; }

        public string Recipient { get; set; }

        public double Amount { get; set; }

        public double Fee { get; set; }

        public string BlockHash { get; set; }

        public DateTime Date => new (this.TimeStamp);
    }
}
