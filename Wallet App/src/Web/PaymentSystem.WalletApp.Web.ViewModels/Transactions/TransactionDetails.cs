namespace PaymentSystem.WalletApp.Web.ViewModels.Transactions
{
    using System;
    using System.Globalization;

    using AutoMapper;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Mapping;

    public class TransactionDetails : IMapFrom<Transaction>
    {
        public string Hash { get; set; }

        public long TimeStamp { get; set; }

        public string Sender { get; set; }

        public string Recipient { get; set; }

        public double Amount { get; set; }

        public double Fee { get; set; }

        public string BlockHash { get; set; }

        [IgnoreMap]
        public string Date => new DateTime(this.TimeStamp)
            .ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
    }
}
