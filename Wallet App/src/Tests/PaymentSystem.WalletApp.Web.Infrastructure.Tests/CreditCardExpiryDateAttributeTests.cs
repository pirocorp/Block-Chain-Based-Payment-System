namespace PaymentSystem.WalletApp.Web.Infrastructure.Tests
{
    using System;
    using System.Linq;
    using ValidationAttributes;
    using Xunit;

    public class CreditCardExpiryDateAttributeTests
    {
        [Theory]
        [InlineData("05/99")]
        [InlineData("05/25")]
        [InlineData("12/99")]
        [InlineData("01/00")]
        public void CreditCardExpiryDateAttributeIsValid(string test)
        {
            var attribute = new CreditCardExpiryDateAttribute();

            Assert.True(attribute.IsValid(test));
        }

        [Theory]
        [InlineData("00/25")]
        [InlineData("05/100")]
        [InlineData("-5/-5")]
        [InlineData("1/25")]
        public void CreditCardExpiryDateAttributeInvalid(string test)
        {
            var attribute = new CreditCardExpiryDateAttribute();

            Assert.False(attribute.IsValid(test));
        }
    }
}
