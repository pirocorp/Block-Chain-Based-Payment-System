namespace PaymentSystem.WalletApp.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Services.Data.Models.Accounts;
    using PaymentSystem.WalletApp.Tests.Mocks;
    using PaymentSystem.WalletApp.Web.Infrastructure;

    using Xunit;

    public class AccountServiceTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IBlockChainGrpcService blockChainGrpcService;

        private readonly IAccountService accountService;

        private readonly string userId;
        private readonly string username;

        public AccountServiceTests()
        {
            MapperHelpers.Load<Account, AccountDerivative>();

            this.dbContext = ApplicationDbContextMocks.Instance;
            this.blockChainGrpcService = BlockChainGrpcServiceMock.Instance;

            this.accountService = new AccountService(this.dbContext, this.blockChainGrpcService);

            this.userId = Guid.NewGuid().ToString();
            this.username = "piroman";
        }

        [Fact]
        public async Task AccountServiceGetPublicKeyWorksCorrectly()
        {
            this.SeedAccounts(new Random().Next(20));

            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                PublicKey = Guid.NewGuid().ToString(),
            };

            this.dbContext.Add(account);
            await this.dbContext.SaveChangesAsync();

            var publicKey = await this.accountService.GetPublicKey(account.Address);

            Assert.Equal(account.PublicKey, publicKey);
        }

        [Fact]
        public async Task AccountServiceGetPublicKeyReturnsNullWhenAccountIsNotPresent()
        {
            this.SeedAccounts(new Random().Next(20));

            var publicKey = await this.accountService.GetPublicKey(Guid.NewGuid().ToString());

            Assert.Null(publicKey);
        }

        [Fact]
        public async Task AccountServiceGetUserIdWorksCorrectly()
        {
            this.SeedAccounts(new Random().Next(20));

            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                PublicKey = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
            };

            this.dbContext.Add(account);
            await this.dbContext.SaveChangesAsync();

            var userId = await this.accountService.GetUserId(account.Address);

            Assert.Equal(account.UserId, userId);
        }

        [Fact]
        public async Task AccountServiceGetUserIdReturnsNullWhenAccountIsNotPresent()
        {
            this.SeedAccounts(new Random().Next(20));

            var userId = await this.accountService.GetUserId(Guid.NewGuid().ToString());

            Assert.Null(userId);
        }

        [Fact]
        public async Task AccountServiceGetAccountReturnsCorrectModel()
        {
            this.SeedAccounts(25);

            var expected = this.dbContext.Accounts
                .OrderBy(a => Guid.NewGuid().ToString())
                .First();

            var actual = await this.accountService.GetAccount<AccountDerivative>(expected.Address);

            Assert.Equal(expected.Address, actual.Address);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.PublicKey, actual.PublicKey);
            Assert.Equal(expected.Balance, actual.Balance);
            Assert.Equal(expected.BlockedBalance, actual.BlockedBalance);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.CreatedOn, actual.CreatedOn);
            Assert.Equal(expected.ModifiedOn, actual.ModifiedOn);
        }

        [Fact]
        public async Task AccountServiceGetLatestAccountsReturnsCorrectResult()
        {
            this.SeedAccounts(25);

            var actual = await this.accountService.GetLatestAccounts<AccountDerivative>();
            var expected = actual.OrderByDescending(a => a.CreatedOn);

            Assert.Equal(WebConstants.DefaultAccountsResultPageSize, actual.Count());
            Assert.Equal(actual, expected);
        }

        [Theory]
        [InlineData(25, 3, 5)]
        [InlineData(18, 3, 10)]
        [InlineData(18, 2, 10)]
        public async Task AccountServiceGetAccountsPaginationWorksFine(int total, int page, int pageSize)
        {
            this.SeedAccounts(total);

            var expected = Math.Max(0, Math.Min(pageSize, total - ((page - 1) * pageSize)));
            var (all, accounts) = await this.accountService.GetAccounts<AccountDerivative>(page, pageSize);

            Assert.Equal(total, all);
            Assert.Equal(expected, accounts.Count());
        }

        [Fact]
        public async Task AccountServiceCreateCreatesAccount()
        {
            this.SeedCurrentUser();

            var result = await this.accountService.Create(this.userId);

            var expected = this.dbContext.Accounts.First(a => a.UserId == this.userId);

            Assert.Equal(expected.UserId, this.userId);
            Assert.Equal(expected.Address, result.Address);
            Assert.Equal(expected.Balance, result.Balance);
            Assert.Equal(expected.PublicKey, result.PublicKey);
        }

        [Fact]
        public async Task AccountServiceBlockFundsCorrectly()
        {
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                Balance = 5000,
                BlockedBalance = 50,
            };

            this.dbContext.Accounts.Add(account);
            await this.dbContext.SaveChangesAsync();

            await this.accountService.BlockFunds(account.Address, 1205);

            Assert.Equal(1255, account.BlockedBalance);
            Assert.Equal(3795, account.Balance);
        }

        [Fact]
        public async Task AccountServiceDepositCorrectly()
        {
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                Balance = 76,
                BlockedBalance = 50,
            };

            this.dbContext.Accounts.Add(account);
            await this.dbContext.SaveChangesAsync();

            await this.accountService.Deposit(account.Address, 1205);

            Assert.Equal(1281, account.Balance);
        }

        [Fact]
        public async Task Withdraw()
        {
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                Balance = 250,
                BlockedBalance = 50,
            };

            this.dbContext.Accounts.Add(account);
            await this.dbContext.SaveChangesAsync();

            await this.accountService.Withdraw(account.Address, 200);

            Assert.Equal(100, account.Balance);
            Assert.Equal(0, account.BlockedBalance);
        }

        [Fact]
        public async Task EditAccountWorksCorrectly()
        {
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                Balance = 250,
                BlockedBalance = 50,
            };

            this.dbContext.Accounts.Add(account);
            await this.dbContext.SaveChangesAsync();

            var model = new EditAccountServiceModel()
            {
                Address = account.Address,
                Name = Guid.NewGuid().ToString(),
            };

            await this.accountService.EditAccount(model);

            Assert.Equal(model.Name, account.Name);
        }

        [Fact]
        public async Task DeleteAccountWorksCorrectly()
        {
            this.SeedCurrentUser();

            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                Balance = 250,
                BlockedBalance = 50,
                UserId = this.userId,
            };

            var accountKey = new AccountKey()
            {
                Address = account.Address,
                UserId = this.userId,
            };

            this.dbContext.Add(accountKey);
            this.dbContext.Accounts.Add(account);
            await this.dbContext.SaveChangesAsync();

            await this.accountService.Delete(account.Address);

            Assert.False(this.dbContext.Accounts.Any());
            Assert.False(this.dbContext.AccountsKeys.Any());
        }

        private void SeedAccounts(int count)
        {
            var rnd = new Random();

            // Add user's accounts
            this.dbContext.AddRange(Enumerable.Range(1, count)
                .Select(i => new Account()
                {
                    Address = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                    PublicKey = Guid.NewGuid().ToString(),
                    Balance = rnd.Next(50, 5000),
                    BlockedBalance = rnd.Next(50, 5000),
                    Name = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.UtcNow.AddDays(-rnd.Next(1, 50)),
                    ModifiedOn = DateTime.UtcNow.AddDays(-rnd.Next(1, 50)),
                }));

            this.dbContext.SaveChanges();
        }

        private void SeedCurrentUser()
        {
            this.dbContext.Add(new ApplicationUser()
            {
                UserName = this.username,
                Id = this.userId,
            });

            this.dbContext.SaveChanges();
        }

        private class AccountDerivative
        {
            public string Address { get; set; }

            public string Name { get; set; }

            public string PublicKey { get; set; }

            public double Balance { get; set; }

            public double BlockedBalance { get; set; }

            public string UserId { get; set; }

            public DateTime CreatedOn { get; set; }

            public DateTime? ModifiedOn { get; set; }
        }
    }
}
