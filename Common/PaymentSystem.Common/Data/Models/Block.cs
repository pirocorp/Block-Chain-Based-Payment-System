namespace PaymentSystem.Common.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;

    [Index(nameof(Height))]
    public class Block
    {
        public Block()
        {
            this.BlockHeader = new BlockHeader();
            this.Transactions = new HashSet<Transaction>();
        }

        // The hash of the block. The hash act as the unique identity of the given block in the block chain.
        [Key]
        public string Hash { get; set; }

        // The sequence amount of blocks.
        public long Height { get; set; }

        public BlockHeader BlockHeader { get; set; }

        // Transactions are collections of transactions that occur.
        // Settled transactions
        public IEnumerable<Transaction> Transactions { get; set; }

        // The creator of the block identified by the public key.
        // Validators get reward from accumulated transaction fees.
        // For now it will be the Block chain app.
        public string Validator { get; set; }
    }
}
