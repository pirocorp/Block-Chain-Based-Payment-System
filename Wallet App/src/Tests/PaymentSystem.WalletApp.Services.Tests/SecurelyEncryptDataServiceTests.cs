namespace PaymentSystem.WalletApp.Services.Tests
{
    using System;
    using System.Text;
    using Data.Models;
    using Xunit;

    public class SecurelyEncryptDataServiceTests
    {
        [Fact]
        public void DataIsEncryptedAndThenDecryptedCorrectly()
        {
            var cardData = new CardData()
            {
                CardHolderName = "Piroman Piromanov",
                CVV = "555",
                ExpiryDate = "05/25",
                CardNumber = "4176449266512499"
            };

            var encryptService = new SecurelyEncryptDataService();
            var saltService = new SaltService();

            var key = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            var salt = saltService.GetSalt();

            var encoded = encryptService.EncryptData(cardData, key, salt);
            var decoded = encryptService.Decrypt<CardData>(encoded, key, salt);

            Assert.Equal(cardData.CardHolderName, decoded.CardHolderName);
            Assert.Equal(cardData.CVV, decoded.CVV);
            Assert.Equal(cardData.ExpiryDate, decoded.ExpiryDate);
            Assert.Equal(cardData.CardNumber, decoded.CardNumber);
        }
    }
}
