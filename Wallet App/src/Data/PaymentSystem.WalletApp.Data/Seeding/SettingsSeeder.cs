using System;
using System.Threading.Tasks;

namespace PaymentSystem.WalletApp.Data.Seeding
{
    using Common;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class SettingsSeeder : ISeeder
    {
        private ApplicationDbContext dbContext;

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            this.dbContext = dbContext;

            if (await this.dbContext.Settings.AnyAsync())
            {
                return;
            }

            await this.SeedSetting(DataConstants.BlockChainAddress, "2DF429FF91627407E48C5F48ED2A66F361515001F113816C0D81378017BC1A1F");
            await this.SeedSetting(DataConstants.BlockChainPublicKey, "2A19FF182CF200CD27A0140312D8129468509B66633B266A85C112B7CB6981C297549C6C6F442257C0F8A4A0CF8C54F1AE01DB39E00A576B36AAE9BD52F3D229");
        }

        private async Task SeedSetting(string key, string value)
        {
            var setting = new Setting()
            {
                Key = key, 
                Value = value,
            };

            await this.dbContext.Settings.AddAsync(setting);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
