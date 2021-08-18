namespace PaymentSystem.WalletApp.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Options;

    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Services.Data.Models.AccountsKeys;
    using PaymentSystem.WalletApp.Services.Implementations;
    using PaymentSystem.WalletApp.Tests.Mocks;
    using PaymentSystem.WalletApp.Web.Infrastructure.Options;

    using Xunit;

    public class AccountsKeyServiceTests
    {
        private readonly IOptions<SecretOptions> secretOptions;
        private readonly ISaltService saltService;
        private readonly ISecurelyEncryptDataService securelyEncryptDataService;
        private readonly ApplicationDbContext dbContext;

        private readonly IAccountsKeyService accountsKeyService;

        public AccountsKeyServiceTests()
        {
            this.secretOptions = SecretOptionsMock.Instance;
            this.saltService = new SaltService();
            this.securelyEncryptDataService = new SecurelyEncryptDataService();

            this.dbContext = ApplicationDbContextMocks.Instance;
            this.accountsKeyService = new AccountsKeyService(
                this.secretOptions,
                this.saltService,
                this.securelyEncryptDataService,
                this.dbContext);
        }

        [Fact]
        public async Task KeyExistsWorksCorrectly()
        {
            var accountKey = new AccountKey()
            {
                Address = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
            };

            this.dbContext.Add(accountKey);
            await this.dbContext.SaveChangesAsync();

            Assert.True(await this.accountsKeyService.KeyExists(accountKey.Address));
        }

        [Fact]
        public async Task DataIsStoredSecurely()
        {
            var model = new StoreAccountKeyServiceModel()
            {
                Address = Guid.NewGuid().ToString(),
                Secret = "My Special Secret!",
            };

            var userId = Guid.NewGuid().ToString();

            await this.accountsKeyService.StoreKeys(model, userId);

            var storedData = this.dbContext.AccountsKeys
                .FirstOrDefault(a => a.Address == model.Address);

            Assert.NotNull(storedData);
            Assert.NotEqual(model.Secret, storedData.Key);
            Assert.Equal(model.Address, storedData.Address);
            Assert.Equal(userId, storedData.UserId);
        }

        [Fact]
        public async Task GetKeyDataReturnsNullKeyIfDataIsNotFoundOrNotBelongingToUser()
        {
            var model = new StoreAccountKeyServiceModel()
            {
                Address = Guid.NewGuid().ToString(),
                Secret = "My Special Secret!",
            };

            var userId = Guid.NewGuid().ToString();

            await this.accountsKeyService.StoreKeys(model, userId);

            var result = await this.accountsKeyService.GetKeyData(model.Address, Guid.NewGuid().ToString());

            Assert.Null(result.Secret);
        }

        [Fact]
        public async Task GetKeyDataReturnsAlreadyStoredData()
        {
            var model = new StoreAccountKeyServiceModel()
            {
                Address = Guid.NewGuid().ToString(),
                Secret = "My Special Secret!",
            };

            var userId = Guid.NewGuid().ToString();

            await this.accountsKeyService.StoreKeys(model, userId);

            var result = await this.accountsKeyService.GetKeyData(model.Address, userId);

            Assert.Equal(model.Secret, result.Secret);
        }
    }
}
