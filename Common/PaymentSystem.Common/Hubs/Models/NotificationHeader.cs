namespace PaymentSystem.Common.Hubs.Models
{
    using PaymentSystem.BlockChain.Services.Mapping;
    using PaymentSystem.Common.Data.Models;

    public class NotificationHeader : IMapFrom<BlockHeader>
    {
        public int Version { get; set; }

        public string PreviousHash { get; set; }

        public string MerkleRoot { get; set; }

        public long TimeStamp { get; set; }

        public int Difficulty { get; set; }

        public string Validator { get; set; }
    }
}
