namespace PaymentSystem.WalletApp.Web.Infrastructure.Tests
{
    using PaymentSystem.WalletApp.Web.Infrastructure.ValidationAttributes;
    using Xunit;

    public class CVVAttributeTests
    {
        [Theory]
        [InlineData("123")]
        [InlineData("1234")]
        [InlineData("000")]
        [InlineData("0000")]
        public void CVVAttributeValidTests(string test)
        {
            var attribute = new CVVAttribute();

            Assert.True(attribute.IsValid(test));
        }

        [Theory]
        [InlineData("asd")]
        [InlineData("12345")]
        [InlineData("12")]
        [InlineData("")]
        [InlineData(null)]
        public void CVVAttributeInValidTests(string test)
        {
            var attribute = new CVVAttribute();

            Assert.False(attribute.IsValid(test));
        }
    }
}
