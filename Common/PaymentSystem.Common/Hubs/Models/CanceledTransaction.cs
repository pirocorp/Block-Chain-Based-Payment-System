namespace PaymentSystem.Common.Hubs.Models
{
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Mapping;

    public class CanceledTransaction : IMapFrom<Transaction>
    {
        public string Hash { get; set; }
    }
}
