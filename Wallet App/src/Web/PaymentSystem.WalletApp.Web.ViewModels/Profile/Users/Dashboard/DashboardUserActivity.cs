namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Users.Dashboard
{
    using System;

    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class DashboardUserActivity : IMapFrom<Activity>
    {
        public long TimeStamp { get; set; }

        public string Description { get; set; }

        public string CounterpartyAddress { get; set; }

        public ActivityStatus Status { get; set; }

        public double Amount { get; set; }

        public string TransactionHash { get; set; }

        [IgnoreMap]
        public DateTime Date => new (this.TimeStamp);
    }
}
