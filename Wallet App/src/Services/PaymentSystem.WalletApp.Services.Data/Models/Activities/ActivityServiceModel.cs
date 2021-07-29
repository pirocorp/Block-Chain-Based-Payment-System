namespace PaymentSystem.WalletApp.Services.Data.Models.Activities
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ActivityServiceModel : IMapTo<Activity>
    {
        public double Amount { get; set; }

        public string CounterpartyAddress { get; set; }

        public string Description { get; set; }

        public ActivityStatus Status { get; set; }

        public string UserId { get; set; }

        public string TransactionHash { get; set; }
    }
}
