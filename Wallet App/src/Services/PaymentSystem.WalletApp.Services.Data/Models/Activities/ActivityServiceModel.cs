namespace PaymentSystem.WalletApp.Services.Data.Models.Activities
{
    using PaymentSystem.WalletApp.Data.Models;

    public class ActivityServiceModel
    {
        public double Amount { get; set; }

        public string CounterpartyAddress { get; set; }

        public string Description { get; set; }

        public ActivityStatus Status { get; set; }

        public string UserId { get; set; }

    }
}
