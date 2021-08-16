namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using Moq;
    using PaymentSystem.WalletApp.Services.Data;

    public static class BlockChainSignalRServiceMock
    {
        public static IBlockChainSignalRService Instance
        {
            get
            {
                var blockChainSignalRServiceMock = new Mock<IBlockChainSignalRService>();

                return blockChainSignalRServiceMock.Object;
            }
        }
    }
}
