namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;

    public static class TempDataMock
    {
        public static TempDataDictionary Instance
        {
            get
            {
                var httpContext = new DefaultHttpContext();
                var tempDataProvider = new Mock<ITempDataProvider>();

                return new TempDataDictionary(httpContext, tempDataProvider.Object);
            }
        }
    }
}
