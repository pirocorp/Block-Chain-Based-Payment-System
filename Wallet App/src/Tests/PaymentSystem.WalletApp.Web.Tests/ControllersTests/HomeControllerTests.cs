namespace PaymentSystem.WalletApp.Web.Tests.ControllersTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Tests.Mocks;
    using PaymentSystem.WalletApp.Web.Controllers;
    using PaymentSystem.WalletApp.Web.ViewModels;
    using PaymentSystem.WalletApp.Web.ViewModels.Home.Index;
    using Xunit;

    public class HomeControllerTests
    {
        [Fact]
        public void ErrorShouldReturnView()
        {
            var homeController = new HomeController(null)
            {
                ControllerContext = ControllerContextMocks.FakeTraceIdentifier("Fake trace"),
            };

            var result = homeController.Error();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task UnAuthenticatedUserOnHomeIndexShouldReturnTestimonials()
        {
            var testimonials = TestimonialsGenerator(5);

            var testimonialsService = TestimonialServiceMock.HomeIndex(testimonials);

            var homeController = new HomeController(testimonialsService)
            {
                ControllerContext = ControllerContextMocks.AnonymousUser(),
            };

            var result = await homeController.Index();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IndexModel>(viewResult.Model);

            Assert.Equal(testimonials.Count(), model.Testimonials.Count());
        }

        [Fact]
        public async Task AuthenticatedUserIsRedirectedToProfileUserDashboard()
        {
            var homeController = new HomeController(null)
            {
                ControllerContext = ControllerContextMocks.LoggedInUser("8deb5b96-b6d8-4baf-bfa8-22b9590429c1", "piro"),
            };

            var result = await homeController.Index();

            Assert.NotNull(result);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public void PrivacyShouldReturnView()
        {
            var homeController = new HomeController(null);
            var result = homeController.Privacy();

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(20)]
        public async Task UnAuthenticatedUserOnHomeIndexShouldReturnCorrectResults(int count)
        {
            MapperHelpers.Load();

            await using var dbContext = ApplicationDbContextMocks.Instance;
            var testimonialService = new TestimonialService(dbContext);

            var input = OriginalTestimonialsGenerator(count).ToList();

            await dbContext.AddRangeAsync(input);
            await dbContext.SaveChangesAsync();

            var homeController = new HomeController(testimonialService)
            {
                ControllerContext = ControllerContextMocks.AnonymousUser(),
            };

            var result = await homeController.Index();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IndexModel>(viewResult.Model);

            var expectedCount = Math.Min(WalletConstants.TestimonialsCountOnHomePage, count);

            Assert.Equal(expectedCount, model.Testimonials.Count());

            foreach (var testimonial in model.Testimonials)
            {
                Assert.Contains(input, t =>
                    t.Name == testimonial.Name
                    && t.Comment == testimonial.Comment
                    && t.UseCase == testimonial.UseCase);
            }
        }

        private static IEnumerable<IndexTestimonialModel> TestimonialsGenerator(int count)
        {
            var testimonials = new List<IndexTestimonialModel>();

            for (var i = 1; i <= count; i++)
            {
                testimonials.Add(new IndexTestimonialModel()
                {
                    Comment = "Comment 2",
                    Name = "Name 2",
                    UseCase = "Use Case 2",
                });
            }

            return testimonials;
        }

        private static IEnumerable<Testimonial> OriginalTestimonialsGenerator(int count)
        {
            var testimonials = new List<Testimonial>();

            for (var i = 1; i <= count; i++)
            {
                testimonials.Add(new Testimonial()
                {
                    Comment = $"Comment {i}",
                    Name = $"Name {i}",
                    UseCase = $"Use Case {i}",
                });
            }

            return testimonials;
        }
    }
}
