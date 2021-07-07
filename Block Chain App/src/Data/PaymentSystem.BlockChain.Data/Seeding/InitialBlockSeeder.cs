namespace PaymentSystem.BlockChain.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using PaymentSystem.BlockChain.Infrastructure;
    using PaymentSystem.Common;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Utilities;

    public class InitialBlockSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Blocks.Any())
            {
                return;
            }

            var systemAddress = dbContext.Settings
                .First(s => s.Key == nameof(AccountKeys.Address))
                .Value;

            var firstTransaction = new Transaction()
            {
                TimeStamp = DateTime.UtcNow.Ticks,
                Sender = "Inception",
                Recipient = systemAddress,
                Amount = Ico.Accounts.Sum(a => a.Balance) + 100_000_000,
                Fee = 0,
            };

            firstTransaction.Hash = BlockChainHashing.GenerateTransactionHash(firstTransaction);

            var transactions = new List<Transaction>()
            {
                firstTransaction,
            };

            var genesisBlock = new Block()
            {
                BlockHeader = new BlockHeader()
                {
                    Version = GlobalConstants.Block.Version,
                    PreviousHash = null,
                    TimeStamp = DateTime.UtcNow.Ticks,
                    Difficulty = GlobalConstants.Block.DefaultDifficulty,
                    Validator = systemAddress,
                },

                Height = 0,
                Transactions = transactions,
            };

            genesisBlock.BlockHeader.MerkleRoot = BlockHelpers.GenerateMerkleRoot(transactions);
            genesisBlock.Hash = BlockHelpers.GenerateBlockHash(genesisBlock);

            transactions.ForEach(t => t.BlockHash = genesisBlock.Hash);

            var account = dbContext.Accounts.First(a => a.Address == systemAddress);
            account.Balance = firstTransaction.Amount;

            await dbContext.AddAsync(genesisBlock);
            await dbContext.SaveChangesAsync();
        }
    }
}
