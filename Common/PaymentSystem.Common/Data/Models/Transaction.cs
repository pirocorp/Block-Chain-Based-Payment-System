#pragma warning disable 8618
namespace PaymentSystem.Common.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Transaction
    {
        // This is the id of the transaction. It is calculated hashing the data in transaction.
        [Key]
        public string Hash { get; set; }

        // Unix timestamps or Epoch timestamps
        public long TimeStamp { get; set; }

        // Address of the sender it is the public key associated with the sender
        public string Sender { get; set; }

        // Address of the recipient it is the public key associated with the recipient
        public string Recipient { get; set; }

        // Amount is send
        public double Amount { get; set; }

        // Fee (cost) associated with this transaction
        public double Fee { get; set; }

        public string BlockHash { get; set; }

        public Block Block { get; set; }
    }
}
