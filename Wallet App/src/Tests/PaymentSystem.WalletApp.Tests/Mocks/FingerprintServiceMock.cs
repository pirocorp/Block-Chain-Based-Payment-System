namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using Moq;
    using PaymentSystem.WalletApp.Services.Data;

    public static class FingerprintServiceMock
    {
        public static IFingerprintService Instance
        {
            get
            {
                var fingerprintServiceMock = new Mock<IFingerprintService>();

                return fingerprintServiceMock.Object;
            }
        }
    }
}
