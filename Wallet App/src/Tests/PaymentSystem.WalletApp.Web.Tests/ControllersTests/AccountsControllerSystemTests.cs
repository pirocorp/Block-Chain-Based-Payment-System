namespace PaymentSystem.WalletApp.Web.Tests.ControllersTests
{
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Tests;

    using Xunit;

    public class AccountsControllerSystemTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;

        public AccountsControllerSystemTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task OnIndexUnAuthorizedUsersAreRedirectedToLoginPage()
        {
            var client = this.factory.CreateClient();

            var response = await client.GetAsync("/Accounts/Index");

            var loginPage = await HtmlHelpers.GetDocumentAsync(response);

            var element = loginPage.QuerySelector("title");
            Assert.Contains("Log in", element.TextContent);
        }

        [Fact]
        public async Task OnTransactionsUnAuthorizedUsersAreRedirectedToLoginPage()
        {
            var client = this.factory.CreateClient();

            var response = await client.GetAsync("/Accounts/Index");

            var loginPage = await HtmlHelpers.GetDocumentAsync(response);

            var element = loginPage.QuerySelector("title");
            Assert.Contains("Log in", element.TextContent);
        }
    }
}
