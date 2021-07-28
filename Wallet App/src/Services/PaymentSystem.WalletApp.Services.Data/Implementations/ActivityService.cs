namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.Activities;

    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext dbContext;

        public ActivityService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddActivity(ActivityServiceModel model)
        {
            var activity = new Activity()
            {
                Amount = model.Amount,
                CounterpartyAddress = model.CounterpartyAddress,
                Description = model.Description,
                Status = model.Status,
                TimeStamp = DateTime.UtcNow.Ticks,
                UserId = model.UserId,
            };

            await this.dbContext.AddAsync(activity);
            await this.dbContext.SaveChangesAsync();
        }
    }
}