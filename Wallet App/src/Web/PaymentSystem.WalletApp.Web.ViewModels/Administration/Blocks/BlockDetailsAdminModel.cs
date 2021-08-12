namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Blocks
{
    using System;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Mapping;

    public class BlockDetailsAdminModel : IMapFrom<Block>
    {
        public string Hash { get; set; }

        public long Height { get; set; }

        public int BlockHeaderVersion { get; set; }

        public string BlockHeaderPreviousHash { get; set; }

        public string BlockHeaderMerkleRoot { get; set; }

        public long BlockHeaderTimeStamp { get; set; }

        public int BlockHeaderDifficulty { get; set; }

        public string BlockHeaderValidator { get; set; }

        public DateTime Date => new (this.BlockHeaderTimeStamp);
    }
}
