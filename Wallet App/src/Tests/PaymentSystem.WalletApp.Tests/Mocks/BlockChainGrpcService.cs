namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using Moq;
    using PaymentSystem.WalletApp.Services;

    public static class BlockChainGrpcServiceMock
    {
        public static IBlockChainGrpcService Instance
        {
            get
            {
                var service = new Mock<IBlockChainGrpcService>();

                return service.Object;
            }
        }
    }
}
