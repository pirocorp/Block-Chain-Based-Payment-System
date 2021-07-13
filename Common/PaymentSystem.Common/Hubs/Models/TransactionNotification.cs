namespace PaymentSystem.Common.Hubs.Models
{
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Mapping;

    public class TransactionNotification : IMapFrom<Transaction>
    {
        public string Hash { get; set; }

        public long TimeStamp { get; set; }

        public string Sender { get; set; }

        public string Recipient { get; set; }

        public double Amount { get; set; }

        public double Fee { get; set; }

        public string BlockHash { get; set; }
    }
}
