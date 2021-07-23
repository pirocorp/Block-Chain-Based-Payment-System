namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.Common.Utilities;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.CreditCards;

    public class CreditCardService : ICreditCardService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISaltService saltService;
        private readonly ISecurelyEncryptDataService securelyEncryptDataService;
        private readonly IFingerprintService fingerprintService;
        private readonly IMapper mapper;

        public CreditCardService(
            ApplicationDbContext dbContext,
            ISaltService saltService,
            ISecurelyEncryptDataService securelyEncryptDataService,
            IFingerprintService fingerprintService,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.saltService = saltService;
            this.securelyEncryptDataService = securelyEncryptDataService;
            this.fingerprintService = fingerprintService;
            this.mapper = mapper;
        }

        public async Task<bool> Exists(string cardId)
            => await this.dbContext.CreditCards.AnyAsync(c => c.Id == cardId);

        public Task<bool> UserOwnsCard(string cardId, string userId)
            => this.dbContext.CreditCards
                .AnyAsync(c => c.Id == cardId && c.UserId == userId);

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

            await using var transaction = await this.dbContext.Database.BeginTransactionAsync();

            try
            {
                await transaction.CreateSavepointAsync("BeforeAddNewCreditCard");

                await this.dbContext.AddAsync(creditCard);
                await this.dbContext.SaveChangesAsync();

                await this.fingerprintService.CreateFingerprint(model.CardNumber);
            }
            catch (Exception)
            {
                await transaction.RollbackToSavepointAsync("BeforeMoreBlogs");
            }

            await transaction.CommitAsync();
        }

        public async Task<IEnumerable<T>> GetCreditCards<T>(string userId)
            => await this.dbContext.CreditCards
                .Where(c => c.UserId == userId)
                .To<T>()
                .ToListAsync();

        public async Task<CreditCardDetailsServiceModel> GetCardInformation(string cardId, byte[] key)
        {
            var card = await this.dbContext.CreditCards.FindAsync(cardId);

            if (card is null)
            {
                return null;
            }

            var cardData = this.GetCardData(key, card);

            var result = this.mapper.Map<CreditCardDetailsServiceModel>(card);

            result.CVV = cardData.CVV;
            result.CardHolderName = cardData.CardHolderName;

            return result;
        }

        public async Task UpdateCardInformation(byte[] key, EditCreditCardServiceModel model)
        {
            var card = await this.dbContext.CreditCards.FindAsync(model.Id);

            if (card is null)
            {
                return;
            }

            card.ExpiryDate = model.ExpiryDate;

            var cardData = this.GetCardData(key, card);

            cardData.CVV = model.CVV;
            cardData.CardHolderName = model.CardHolderName;
            card.SecurityStamp = this.saltService.GetSaltHex();

            var salt = card.SecurityStamp.HexToBytes();
            var encryptedCardData = this.securelyEncryptDataService.EncryptDataHex(cardData, key, salt);

            card.CardData = encryptedCardData;

            this.dbContext.Update(card);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteCreditCard(string cardId)
        {
            var creditCard = await this.dbContext.CreditCards.FindAsync(cardId);

            if (creditCard is null)
            {
                return;
            }

            this.dbContext.CreditCards.Remove(creditCard);
            await this.dbContext.SaveChangesAsync();
        }

        private CardData GetCardData(byte[] key, CreditCard card)
        {
            var salt = card.SecurityStamp.HexToBytes();
            var cardBytes = card.CardData.HexToBytes();
            var cardData = this.securelyEncryptDataService.Decrypt<CardData>(cardBytes, key, salt);

            return cardData;
        }
    }
}
