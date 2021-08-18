namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using Microsoft.Extensions.Options;
    using Moq;
    using PaymentSystem.WalletApp.Web.Infrastructure.Options;

    public static class SecretOptionsMock
    {
        public static IOptions<SecretOptions> Instance
        {
            get
            {
                var secretOptions = new Mock<IOptions<SecretOptions>>();

                secretOptions
                    .Setup(s => s.Value)
                    .Returns(new SecretOptions()
                    {
                        Key = "Super Secret",
                    });

                return secretOptions.Object;
            }
        }
    }
}
