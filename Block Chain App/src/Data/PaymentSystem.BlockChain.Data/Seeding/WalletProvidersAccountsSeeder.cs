namespace PaymentSystem.BlockChain.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using PaymentSystem.BlockChain.Data.Models;
    using PaymentSystem.BlockChain.Infrastructure;
    using PaymentSystem.Common;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Utilities;

    public class WalletProvidersAccountsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Blocks.Count() > 1)
            {
                return;
            }

            var systemAddress = dbContext.Settings
                .First(s => s.Key == nameof(AccountKeys.Address))
                .Value;

            var systemAccount = dbContext.Accounts.First(a => a.Address == systemAddress);

            var lastBlock = dbContext.Blocks.OrderBy(x => x.Height).Last();

            var transactions = new List<Transaction>();
            var accounts = new List<Account>();

            foreach (var icoAccount in WalletProviders.Accounts)
            {
                var transaction = new Transaction()
                {
                    TimeStamp = DateTime.UtcNow.Ticks,
                    Sender = systemAddress,
                    Recipient = icoAccount.Address.ToUpper(),
                    Amount = icoAccount.Balance,
                    Fee = 0,
                };

                transaction.Hash = BlockChainHashing.GenerateTransactionHash(transaction);
                transactions.Add(transaction);

                systemAccount.Balance -= icoAccount.Balance;

                var account = new Account()
                {
                    Address = icoAccount.Address.ToUpper(),
                    Balance = icoAccount.Balance,
                    PublicKey = icoAccount.PublicKey,
                    IsMarketMaker = true,
                };

                accounts.Add(account);
            }

            var block = new Block()
            {
                BlockHeader = new BlockHeader()
                {
                    Version = GlobalConstants.Block.Version,
                    PreviousHash = lastBlock.Hash,
                    TimeStamp = DateTime.UtcNow.Ticks,
                    Difficulty = GlobalConstants.Block.DefaultDifficulty,
                    Validator = systemAddress,
                },

                Height = 1,
                Transactions = transactions,
            };

            block.BlockHeader.MerkleRoot = BlockHelpers.GenerateMerkleRoot(transactions);
            block.Hash = BlockHelpers.GenerateBlockHash(block);

            await dbContext.AddAsync(block);
            await dbContext.AddRangeAsync(accounts);
            await dbContext.SaveChangesAsync();
        }
    }
}
