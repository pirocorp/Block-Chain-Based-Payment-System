namespace PaymentSystem.WalletApp.Web.Tests.ControllersTests
{
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Tests;
    using Xunit;

    public class HomeControllerSystemTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;

        public HomeControllerSystemTest(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task PrivacyShouldReturnCorrectView()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Home/Privacy");

            // Assert
            Assert.True(response.IsSuccessStatusCode);

            var privacyHtml = await HtmlHelpers.GetDocumentAsync(response);
            var element = privacyHtml.QuerySelector("h1");
            Assert.Contains("Privacy Policy", element.TextContent);
        }
    }
}
