namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Moq;

    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.ViewModels.Home.Index;

    public static class TestimonialServiceMock
    {
        public static ITestimonialService HomeIndex(IEnumerable<IndexTestimonialModel> testimonials)
        {
            var statisticsServiceMock = new Mock<ITestimonialService>();

            statisticsServiceMock
                .Setup(s => s.GetTestimonials<IndexTestimonialModel>(It.IsAny<int>()))
                .Returns(Task.FromResult(testimonials));

            return statisticsServiceMock.Object;
        }
    }
}
