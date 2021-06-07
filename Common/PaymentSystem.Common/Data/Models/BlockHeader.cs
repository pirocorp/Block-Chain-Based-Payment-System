#pragma warning disable 8618
namespace PaymentSystem.Common.Data.Models
{
    public class BlockHeader
    {
        public int Version { get; set; }

        // PreviousHash is the hash of the previous block.
        // For root block it will be null
        public string PreviousHash { get; set; }

        // The root hash of Merkle Tree (Hash Tree).
        // The Hash Tree made from hashes of transactions in the block.
        public string MerkleRoot { get; set; }

        // Unix timestamps or Epoch timestamps of time of block creation
        public long TimeStamp { get; set; }

        // Will use constant difficulty but for now is here
        public int Difficulty { get; set; }

        // The creator of the block identified by the public key.
        // Validators get reward from accumulated transaction fees.
        public string Validator { get; set; }
    }
}
