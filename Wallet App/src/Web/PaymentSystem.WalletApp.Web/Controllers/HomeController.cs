namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.Areas.Profile.Controllers;
    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;
    using PaymentSystem.WalletApp.Web.ViewModels;
    using PaymentSystem.WalletApp.Web.ViewModels.Home.Index;

    using Activity = System.Diagnostics.Activity;

    public class HomeController : BaseController
    {
        private readonly ITestimonialService testimonialService;

        public HomeController(ITestimonialService testimonialService)
        {
            this.testimonialService = testimonialService;
        }

        public async Task<IActionResult> Index()
        {
            if (this.User.Identity?.IsAuthenticated ?? false)
            {
                var controller = ControllerHelpers.GetControllerName<UsersController>();
                var area = ControllerHelpers.GetAreaName(typeof(ProfileController));

                return this.RedirectToAction(nameof(UsersController.Dashboard), controller, new {area = area });
            }

            var model = new IndexModel
            {
                Testimonials = await this.testimonialService
                    .GetTestimonials<IndexTestimonialModel>(WalletConstants.TestimonialsCountOnHomePage),
            };

            return this.View(model);
        }

        public IActionResult Privacy() => this.View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
