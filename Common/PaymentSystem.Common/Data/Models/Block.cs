#pragma warning disable 8618
namespace PaymentSystem.Common.Data.Models
{
    using System.Collections.Generic;

    public class Block
    {
        public Block()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        // The hash of the block. The hash act as the unique identity of the given block in the block chain.
        public string Hash { get; set; }

        // The sequence amount of blocks.
        public long Height { get; set; }

        public BlockHeader BlockHeader { get; set; }

        // Transactions are collections of transactions that occur.
        // Settled transactions
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
