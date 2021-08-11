namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Blocks;

    using static Infrastructure.WebConstants;

    public class BlocksController : AdministrationController
    {
        private readonly IBlockService blockService;

        public BlocksController(IBlockService blockService)
        {
            this.blockService = blockService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var currentPageSize = DefaultBlocksResultPageSize;

            var (totalPages, blocks) = await this.Pagination(
                this.blockService.GetBlocks<BlockListingAdminModel>,
                page,
                currentPageSize);

            var model = new BlockIndexAdminViewModel()
            {
                Blocks = blocks,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return this.View(model);
        }

        public async Task<IActionResult> Details(string hash)
        {
            return this.Ok(hash);
        }
    }
}
