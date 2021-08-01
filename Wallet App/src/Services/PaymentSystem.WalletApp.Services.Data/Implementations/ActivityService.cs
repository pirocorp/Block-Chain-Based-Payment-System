namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.Activities;

    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public ActivityService(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<bool> Exists(string transactionHash)
            => await this.dbContext.Activities.AnyAsync(a => a.TransactionHash == transactionHash);

        public async Task AddActivity(ActivityServiceModel model)
        {
            var activity = this.mapper.Map<Activity>(model);

            await this.dbContext.AddAsync(activity);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task SetActivityStatus(string transactionHash, ActivityStatus status)
        {
            var activity = await this.dbContext.Activities
                .FirstOrDefaultAsync(a => a.TransactionHash == transactionHash);

            activity.Status = status;

            await this.dbContext.SaveChangesAsync();
        }
    }
}