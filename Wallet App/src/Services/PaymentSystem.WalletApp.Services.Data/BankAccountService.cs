namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.BankAccounts;

    public class BankAccountService : IBankAccountService
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;

        public BankAccountService(
            IMapper mapper,
            ApplicationDbContext dbContext)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task<bool> Exists(string id)
            => await this.dbContext.BankAccounts.AnyAsync(a => a.Id == id);

        public async Task<bool> UserOwnsCard(string id, string userId)
            => await this.dbContext.BankAccounts.AnyAsync(a => a.Id == id && a.UserId == userId);

        public async Task<T> GetCardInformation<T>(string id)
            => await this.dbContext.BankAccounts
                .Where(a => a.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task AddAccount(AddBankAccountServiceModel model, string userId)
        {
            var bankAccount = this.mapper.Map<BankAccount>(model);
            bankAccount.UserId = userId;

            await this.dbContext.AddAsync(bankAccount);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAccounts<T>(string userId)
            => await this.dbContext.BankAccounts
                .Where(a => a.UserId == userId)
                .To<T>()
                .ToListAsync();

        public async Task DeleteAccount(string id)
        {
            var account = await this.dbContext.BankAccounts.FindAsync(id);

            this.dbContext.Remove(account);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
