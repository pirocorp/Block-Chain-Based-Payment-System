namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using Moq;
    using PaymentSystem.WalletApp.Services.Data;

    public static class CreditCardServiceMock
    {
        public static ICreditCardService Instance
        {
            get
            {
                var creditCardServiceMock = new Mock<ICreditCardService>();

                return creditCardServiceMock.Object;
            }
        }
    }
}
