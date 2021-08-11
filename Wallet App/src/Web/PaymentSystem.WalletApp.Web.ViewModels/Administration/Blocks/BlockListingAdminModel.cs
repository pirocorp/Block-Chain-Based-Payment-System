namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Blocks
{
    using System;
    using Infrastructure.Helpers;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Mapping;

    public class BlockListingAdminModel : IMapFrom<Block>
    {
        public string Hash { get; set; }

        public long Height { get; set; }

        public int TransactionsCount { get; set; }

        public long BlockHeaderTimeStamp { get; set; }

        public DateTime Date => new (this.BlockHeaderTimeStamp);

        public string FriendlyHash => AddressHelpers.FriendlyAddress(this.Hash);
    }
}
