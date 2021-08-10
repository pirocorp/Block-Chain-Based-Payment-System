namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.Infrastructure;
    using PaymentSystem.WalletApp.Web.ViewModels.Activities;
    using PaymentSystem.WalletApp.Web.ViewModels.Activities.Index;

    [Authorize]
    public class ActivitiesController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IActivityService activityService;

        public ActivitiesController(
            UserManager<ApplicationUser> userManager,
            IActivityService activityService)
        {
            this.userManager = userManager;
            this.activityService = activityService;
        }

        public async Task<IActionResult> Index(int page = 1, string dateRange = "")
        {
            page = Math.Max(page, 1);

            var userId = this.userManager.GetUserId(this.User);

            var (total, activities) = await this.activityService
                .GetUserActivities<DetailsActivityModel>(userId, page, WebConstants.DefaultActivitiesResultPageSize, dateRange);

            var model = new ActivitiesIndexViewModel()
            {
                Activities = activities,
                DateRange = dateRange,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(total / (double) WebConstants.DefaultActivitiesResultPageSize),
            };

            return this.View(model);
        }
    }
}
