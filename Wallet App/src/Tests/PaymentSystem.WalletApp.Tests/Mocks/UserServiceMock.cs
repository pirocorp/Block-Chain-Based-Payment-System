namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using Moq;

    using PaymentSystem.WalletApp.Services.Data;

    public static class UserServiceMock
    {
        public static IUserService Instance
        {
            get
            {
                var userServiceMock = new Mock<IUserService>();

                userServiceMock
                    .Setup(x => x.SendCoins(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.Is<double>(a => a < 1),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                    .ReturnsAsync(false);

                userServiceMock
                    .Setup(x => x.SendCoins(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.Is<double>(a => a >= 1),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                    .ReturnsAsync(true);

                return userServiceMock.Object;
            }
        }
    }
}
