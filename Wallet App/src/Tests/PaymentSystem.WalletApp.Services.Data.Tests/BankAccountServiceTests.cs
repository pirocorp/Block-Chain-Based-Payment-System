namespace PaymentSystem.WalletApp.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Services.Data.Models.BankAccounts;
    using PaymentSystem.WalletApp.Tests.Mocks;
    using PaymentSystem.WalletApp.Web.ViewModels.Profile.Financials.Profile;

    using Xunit;

    public class BankAccountServiceTests
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;

        private readonly IBankAccountService bankAccountService;

        public BankAccountServiceTests()
        {
            MapperHelpers.Load();

            this.mapper = MapperHelpers.Instance;
            this.dbContext = ApplicationDbContextMocks.Instance;

            this.bankAccountService = new BankAccountService(this.mapper, this.dbContext);
        }

        [Fact]
        public async Task GetAccountInformationWorksCorrectly()
        {
            var expected = new BankAccount()
            {
                IBAN = Guid.NewGuid().ToString(),
                IsApproved = true,
            };

            this.dbContext.Add(expected);
            await this.dbContext.SaveChangesAsync();

            var actual = await this.bankAccountService
                .GetAccountInformation<ProfileDeleteBankAccountModel>(expected.Id);

            Assert.Equal(expected.IBAN, actual.IBAN);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.IsApproved, actual.IsApproved);
        }

        [Fact]
        public async Task AddAccountAddAccountToDatabase()
        {
            var model = new AddBankAccountServiceModel()
            {
                AccountName = Guid.NewGuid().ToString(),
                BankName = Guid.NewGuid().ToString(),
                Country = Guid.NewGuid().ToString(),
                IBAN = Guid.NewGuid().ToString(),
                Swift = Guid.NewGuid().ToString(),
            };

            var userId = Guid.NewGuid().ToString();

            await this.bankAccountService.AddAccount(model, userId);

            var actual = this.dbContext.BankAccounts.First(b => b.IBAN == model.IBAN);

            Assert.Equal(model.AccountName, actual.AccountHolderName);
            Assert.Equal(model.BankName, actual.BankName);
            Assert.Equal(model.Country, actual.Country);
            Assert.Equal(model.IBAN, actual.IBAN);
            Assert.Equal(model.Swift, actual.Swift);
            Assert.Equal(userId, actual.UserId);
        }

        [Fact]
        public async Task GetAccountsReturnAllUserAccounts()
        {
            var userId = Guid.NewGuid().ToString();

            this.dbContext.AddRange(Enumerable.Range(1, 3)
                .Select(a => new BankAccount() { UserId = userId }));

            this.dbContext.AddRange(Enumerable.Range(1, 5)
                .Select(a => new BankAccount()));

            await this.dbContext.SaveChangesAsync();

            var accounts = await this.bankAccountService
                .GetAccounts<ProfileBankAccountModel>(userId);

            Assert.Equal(3, accounts.Count());
        }

        [Fact]
        public async Task DeleteAccountRemovesCorrectAccount()
        {
            this.dbContext.AddRange(Enumerable.Range(1, 5)
                .Select(a => new BankAccount()));

            await this.dbContext.SaveChangesAsync();

            var deleteMe = this.dbContext.BankAccounts
                .OrderByDescending(a => Guid.NewGuid().ToString())
                .First();

            await this.bankAccountService.DeleteAccount(deleteMe.Id);

            Assert.False(this.dbContext.BankAccounts.Any(a => a.Id == deleteMe.Id));
        }
    }
}
