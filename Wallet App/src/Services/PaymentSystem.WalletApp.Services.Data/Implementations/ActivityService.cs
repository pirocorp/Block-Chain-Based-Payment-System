namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
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

        public async Task AddActivity(ActivityServiceModel model)
        {
            var activity = this.mapper.Map<Activity>(model);

            await this.dbContext.AddAsync(activity);
            await this.dbContext.SaveChangesAsync();
        }
    }
}