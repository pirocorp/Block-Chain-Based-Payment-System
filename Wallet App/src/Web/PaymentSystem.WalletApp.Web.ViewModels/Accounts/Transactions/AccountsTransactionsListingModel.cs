namespace PaymentSystem.WalletApp.Web.ViewModels.Accounts.Transactions
{
    using System;

    using AutoMapper;
    using Infrastructure.Helpers;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Mapping;

    using static PaymentSystem.WalletApp.Web.Infrastructure.WebConstants;

    public class AccountsTransactionsListingModel : IMapFrom<Transaction>
    {
        public string Hash { get; set; }

        public string Sender { get; set; }

        public string Recipient { get; set; }

        public double Amount { get; set; }

        public double Fee { get; set; }

        public long TimeStamp { get; set; }

        [IgnoreMap]
        public string CurrentAccountAddress { get; set; }

        [IgnoreMap]
        public DateTime Date => new (this.TimeStamp);

        [IgnoreMap]
        public string CounterpartyAddress
            => this.Sender == this.CurrentAccountAddress
                ? AddressHelpers.FriendlyAddress(this.Recipient)
                : AddressHelpers.FriendlyAddress(this.Sender);

        [IgnoreMap]
        public string Description
            => this.Sender == this.CurrentAccountAddress
                ? string.Format(SendDescription, this.Amount.ToString("F2"), AddressHelpers.FriendlyAddress(this.Recipient))
                : string.Format(ReceiveDescription, this.Amount.ToString("F2"), AddressHelpers.FriendlyAddress(this.Sender));

        [IgnoreMap]
        public double Total
            => this.Sender == this.CurrentAccountAddress
                ? -(this.Amount + this.Fee)
                : this.Amount;
    }
}
