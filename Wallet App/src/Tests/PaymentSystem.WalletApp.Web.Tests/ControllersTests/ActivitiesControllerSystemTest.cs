namespace PaymentSystem.WalletApp.Web.Tests.ControllersTests
{
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Tests;

    using Xunit;

    public class ActivitiesControllerSystemTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;

        public ActivitiesControllerSystemTest(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task OnIndexUnAuthorizedUsersAreRedirectedToLoginPage()
        {
            var client = this.factory.CreateClient();

            var response = await client.GetAsync("/Activities");

            var loginPage = await HtmlHelpers.GetDocumentAsync(response);

            var element = loginPage.QuerySelector("title");
            Assert.Contains("Log in", element.TextContent);
        }
    }
}
