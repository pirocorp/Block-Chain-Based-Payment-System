namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Users;

    using static Infrastructure.WebConstants;

    public class UsersController : AdministrationController
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var currentPageSize = DefaultUsersResultPageSize;

            var (totalPages, users) = await this.Pagination(
                this.userService.GetUsers<UserListingAdminModel>,
                page,
                currentPageSize);

            var model = new UsersIndexAdminViewModel()
            {
                CurrentPage = page,
                TotalPages = totalPages,
                Users = users,
            };

            return this.View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var model = await this.userService.GetUser<UserDetailsAdminModel>(id);

            return this.View(model);
        }
    }
}
