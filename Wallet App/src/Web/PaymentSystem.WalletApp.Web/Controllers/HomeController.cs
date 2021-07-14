namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Common;
    using Infrastructure.Helpers;
    using PaymentSystem.WalletApp.Web.ViewModels;

    using Microsoft.AspNetCore.Mvc;
    using Services.Data;
    using ViewModels.Home;
    using ViewModels.Home.Index;

    public class HomeController : BaseController
    {
        private readonly ITestimonialService testimonialService;

        public HomeController(ITestimonialService testimonialService)
        {
            this.testimonialService = testimonialService;
        }

        public async Task<IActionResult> Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var controller = ControllerHelpers.GetControllerName<UsersController>();

                return this.Redirect($"/{controller}/{nameof(UsersController.Dashboard)}");
            }

            var model = new IndexModel
            {
                Testimonials = await this.testimonialService
                    .GetTestimonials<IndexTestimonialModel>(WalletConstants.TestimonialsCountOnHomePage)
            };

            return this.View(model);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
