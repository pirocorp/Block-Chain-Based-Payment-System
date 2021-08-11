namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Blocks
{
    using System.Collections.Generic;

    public class BlockListingAdminPartialModel
    {
        public string Title { get; set; }

        public IEnumerable<BlockListingAdminModel> Blocks { get; set; }
    }
}
