namespace PaymentSystem.BlockChain.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using PaymentSystem.BlockChain.Data.Models;
    using PaymentSystem.Common.Utilities;

    // It will seed system account.
    public class SystemSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (!dbContext.Settings.Any(s => s.Key == nameof(AccountKeys.Secret)))
            {
                var systemAccountKeys = AccountHelpers.CreateAccount();

                var settings = new List<Setting>()
                {
                    new ()
                    {
                        Key = nameof(AccountKeys.Secret),
                        Value = systemAccountKeys.Secret.BigIntegerToHex(),
                    },
                    new ()
                    {
                        Key = nameof(AccountKeys.Address),
                        Value = systemAccountKeys.Address,
                    },
                    new ()
                    {
                        Key = nameof(AccountKeys.PublicKey),
                        Value = systemAccountKeys.PublicKey.PublicKeyToString(),
                    },
                };

                var account = new Account()
                {
                    Address = systemAccountKeys.Address,
                    PublicKey = systemAccountKeys.PublicKey.PublicKeyToString(),
                };

                await dbContext.Settings.AddRangeAsync(settings);
                await dbContext.Accounts.AddAsync(account);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
