namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using Moq;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Models.Transactions;

    public static class TransferServiceMock
    {
        public static ITransferService Instance
        {
            get
            {
                var transferService = new Mock<ITransferService>();

                transferService
                    .Setup(x
                        => x.DepositToAccount(
                            It.IsAny<string>(),
                            It.Is<DepositServiceModel>(d => d.Amount <= 0)))
                    .ReturnsAsync(false);

                transferService
                    .Setup(x
                        => x.DepositToAccount(
                            It.IsAny<string>(),
                            It.Is<DepositServiceModel>(d => d.Amount > 0)))
                    .ReturnsAsync(true);

                return transferService.Object;
            }
        }
    }
}
