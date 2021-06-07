namespace PaymentSystem.BlockChain.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using PaymentSystem.BlockChain.Data;

    public interface ISeeder
    {
        Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}
