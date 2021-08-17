namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using Moq;
    using PaymentSystem.WalletApp.Services;

    public static class SaltServiceMock
    {
        public static ISaltService Instance
        {
            get
            {
                var saltService = new Mock<ISaltService>();

                return saltService.Object;
            }
        }
    }
}
