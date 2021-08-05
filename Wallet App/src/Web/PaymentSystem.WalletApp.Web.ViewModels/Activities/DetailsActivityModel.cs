namespace PaymentSystem.WalletApp.Web.ViewModels.Activities
{
    using System;

    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class DetailsActivityModel : IMapFrom<Activity>
    {
        public double Amount { get; set; }

        public string CounterpartyAddress { get; set; }

        public string Description { get; set; }

        public ActivityStatus Status { get; set; }

        public long TimeStamp { get; set; }

        public string TransactionHash { get; set; }

        [IgnoreMap]
        public DateTime Date => new(this.TimeStamp);
    }
}
