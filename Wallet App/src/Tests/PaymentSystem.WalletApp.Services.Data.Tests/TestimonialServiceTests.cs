namespace PaymentSystem.WalletApp.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Tests.Mocks;
    using PaymentSystem.WalletApp.Web.ViewModels.Home.Index;

    using Xunit;

    public class TestimonialServiceTests
    {
        [Fact]
        public async Task GetTestimonialsReturnsCorrectTestimonials()
        {
            MapperHelpers.Load();

            await using var dbContext = ApplicationDbContextMocks.Instance;
            var testimonialService = new TestimonialService(dbContext);

            var input = TestimonialsGenerator(10);

            await dbContext.AddRangeAsync(input);
            await dbContext.SaveChangesAsync();

            var count = 5;
            var testimonials = await testimonialService
                .GetTestimonials<IndexTestimonialModel>(count);

            foreach (var testimonial in testimonials)
            {
                Assert.Contains(input, t =>
                    t.Name == testimonial.Name
                    && t.Comment == testimonial.Comment
                    && t.UseCase == testimonial.UseCase);
            }

            Assert.Equal(count, testimonials.Count());
        }

        private static IEnumerable<Testimonial> TestimonialsGenerator(int count)
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
