namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Blocks
{
    using System.Collections.Generic;

    public class BlockIndexAdminViewModel
    {
        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<BlockListingAdminModel> Blocks { get; set; }
    }
}
