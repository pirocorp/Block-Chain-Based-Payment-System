namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using System;
    using Moq;
    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.WalletApp.Services;

    public static class BlockChainGrpcServiceMock
    {
        public static IBlockChainGrpcService Instance
        {
            get
            {
                var service = new Mock<IBlockChainGrpcService>();

                service
                    .Setup(x => x.CreateAccount())
                    .ReturnsAsync(new AccountCreationResponse()
                    {
                        Address = Guid.NewGuid().ToString(),
                        Balance = 50,
                        PublicKey = Guid.NewGuid().ToString(),
                        Secret = Guid.NewGuid().ToString(),
                    });

                return service.Object;
            }
        }
    }
}
