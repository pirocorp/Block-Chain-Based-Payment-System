namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Services.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.BankAccounts;
    using PaymentSystem.WalletApp.Services.Data.Models.CreditCards;

    public class UserService : IUserService
    {
        private const int BankAccountLastDigitCount = 4;
        private const int CoinAccountLastDigitCount = 12;

        private readonly ApplicationDbContext dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<T> GetUser<T>(string id)
            => await this.dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<T>> GetBankAccounts<T>(string id)
            => await this.dbContext.BankAccounts
                .Where(a => a.UserId == id)
                .To<T>()
                .ToListAsync();

        public async Task<IEnumerable<T>> GetCreditCards<T>(string id)
            => await this.dbContext.CreditCards
                .Where(a => a.UserId == id)
                .To<T>()
                .ToListAsync();

        public async Task<IEnumerable<SelectListItem>> GetPaymentMethods(string userId, PaymentMethod paymentMethod)
            => paymentMethod switch
            {
                PaymentMethod.CreditOrDebitCard => (await this.GetCreditCards<CreditCardSelectListModel>(userId))
                    .Select(c => new SelectListItem($"XXXX - {c.LastFourDigits}", c.Id))
                    .ToList(),

                PaymentMethod.BankAccount => (await this.GetBankAccounts<BankAccountsSelectListModel>(userId))
                    .Select(b => new SelectListItem($"{b.BankName} - {b.IBAN[^BankAccountLastDigitCount..]}", b.Id))
                    .ToList(),

                _ => new List<SelectListItem>(),
            };

        public async Task<IEnumerable<SelectListItem>> GetCoinAccounts(string userId)
            => (await this.dbContext.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    a.Address,
                })
                .ToListAsync())
                .Select(a => new SelectListItem($"XXXX - {a.Address[^CoinAccountLastDigitCount..]}", a.Address))
                .ToList();
    }
}
