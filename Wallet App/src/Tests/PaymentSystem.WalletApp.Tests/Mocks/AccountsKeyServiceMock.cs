namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using System;
    using Data.Models;
    using Moq;

    using PaymentSystem.WalletApp.Services.Data;

    public static class AccountsKeyServiceMock
    {
        public static IAccountsKeyService Instance
        {
            get
            {
                var accountKeyService = new Mock<IAccountsKeyService>();

                accountKeyService
                    .Setup(x => x.GetKeyData(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new KeyData()
                    {
                        Secret = Guid.NewGuid().ToString(),
                    });

                accountKeyService
                    .Setup(x => x.KeyExists(It.Is<string>(s => s == "Exists")))
                    .ReturnsAsync(true);

                accountKeyService
                    .Setup(x => x.KeyExists(It.IsAny<string>()))
                    .ReturnsAsync(false);

                return accountKeyService.Object;
            }
        }
    }
}
