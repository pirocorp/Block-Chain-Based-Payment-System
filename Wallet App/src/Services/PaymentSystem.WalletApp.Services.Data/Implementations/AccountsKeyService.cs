namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.Options;
    using PaymentSystem.Common.Utilities;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.AccountsKeys;
    using PaymentSystem.WalletApp.Web.Infrastructure.Options;
    using WalletApp.Data;

    public class AccountsKeyService : IAccountsKeyService
    {
        private readonly IOptions<SecretOptions> secretOptions;
        private readonly ISaltService saltService;
        private readonly ISecurelyEncryptDataService securelyEncryptDataService;
        private readonly ApplicationDbContext dbContext;

        public AccountsKeyService(
            IOptions<SecretOptions> secretOptions,
            ISaltService saltService,
            ISecurelyEncryptDataService securelyEncryptDataService,
            ApplicationDbContext dbContext)
        {
            this.secretOptions = secretOptions;
            this.saltService = saltService;
            this.securelyEncryptDataService = securelyEncryptDataService;
            this.dbContext = dbContext;
        }

        public async Task StoreKeys(StoreAccountKeyServiceModel model, string userId)
        {
            var salt = this.saltService.GetSalt();

            var keyData = new KeyData()
            {
                Secret = model.Secret,
            };

            var key = this.secretOptions.Value.Key.ToByteArray();

            var accountKey = new AccountKey()
            {
                UserId = userId,
                Address = model.Address,
                SecurityStamp = salt.BytesToHex(),
                Key = this.securelyEncryptDataService.EncryptDataHex(keyData, key, salt),
            };

            await this.dbContext.AddAsync(accountKey);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
