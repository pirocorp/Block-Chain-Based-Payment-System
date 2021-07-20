namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.Common.Utilities;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models;

    public class CreditCardService : ICreditCardService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISaltService saltService;
        private readonly ISecurelyEncryptDataService securelyEncryptDataService;

        public CreditCardService(
            ApplicationDbContext dbContext,
            ISaltService saltService,
            ISecurelyEncryptDataService securelyEncryptDataService)
        {
            this.dbContext = dbContext;
            this.saltService = saltService;
            this.securelyEncryptDataService = securelyEncryptDataService;
        }

        public async Task AddCreditCard(AddCreditCardServiceModel model, string userId, byte[] key)
        {
            var salt = this.saltService.GetSalt();

            var cardData = new CardData()
            {
                CardHolderName = model.CardHolderName,
                CardNumber = model.CardNumber,
                CVV = model.CVV,
            };

            var encryptedCardData = this.securelyEncryptDataService.EncryptDataHex(cardData, key, salt);

            var creditCard = new CreditCard()
            {
                IsCredit = model.IsCredit,
                CardType = model.CardType,
                SecurityStamp = salt.BytesToHex(),
                UserId = userId,
                CardData = encryptedCardData,
                ExpiryDate = model.ExpiryDate,
                LastFourDigits = model.CardNumber[^4..],
            };

            await this.dbContext.AddAsync(creditCard);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetCreditCards<T>(string userId)
            => await this.dbContext.CreditCards
                .Where(c => c.UserId == userId)
                .To<T>()
                .ToListAsync();
    }
}
