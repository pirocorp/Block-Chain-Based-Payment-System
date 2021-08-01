namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Common;

    public class SettingService : ISettingService
    {
        private readonly ApplicationDbContext dbContext;

        public SettingService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> GetSetting(string key)
            => (await this.dbContext.Settings.FirstOrDefaultAsync(s => s.Key == key))
                ?.Value ?? string.Empty;

        public async Task<string> GetBlockChainAddress()
            => await this.GetSetting(DataConstants.BlockChainAddress);

        public async Task<string> GetBlockPublicKey()
            => await this.GetSetting(DataConstants.BlockChainPublicKey);
    }
}
