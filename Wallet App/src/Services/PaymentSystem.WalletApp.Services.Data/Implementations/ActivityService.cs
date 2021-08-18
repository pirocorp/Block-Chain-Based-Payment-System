namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;
    using PaymentSystem.Common.Mapping;
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
            if (model is null)
            {
                return;
            }

            var activity = this.mapper.Map<Activity>(model);
            activity.TimeStamp = DateTime.UtcNow.Ticks;

            await this.dbContext.AddAsync(activity);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task SetActivityStatus(string transactionHash, ActivityStatus status)
        {
            var activity = await this.GetActivity(transactionHash);

            if (activity is null)
            {
                return;
            }

            activity.Status = status;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<(int Total, IEnumerable<T> Activities)> GetUserActivities<T>(
            string userId,
            int page,
            int pageSize,
            string dateRange = "")
        {
            var activitiesQuery = this.dbContext.Activities
                .Where(a => a.UserId == userId);

            var dates = dateRange
                .Split("-", StringSplitOptions.RemoveEmptyEntries)
                .Select(d => DateTime.Parse(d, CultureInfo.InvariantCulture)).ToList();

            if (dates.Any())
            {
                var startDate = dates[0];
                var endDate = dates[1];

                endDate = endDate.AddDays(1);

                var startTimeStamp = startDate.Ticks;
                var endTimeStamp = endDate.Ticks;

                activitiesQuery = activitiesQuery
                    .Where(a => a.TimeStamp >= startTimeStamp && a.TimeStamp < endTimeStamp);
            }

            var total = await activitiesQuery.CountAsync();

            var activities = await activitiesQuery
                .OrderByDescending(a => a.TimeStamp)
                .To<T>()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, activities);
        }

        public async Task ReturnBlockedAmount(string transactionHash)
        {
            if (string.IsNullOrWhiteSpace(transactionHash))
            {
                return;
            }

            var activity = await this.dbContext.Activities
                .FirstAsync(a => a.TransactionHash == transactionHash);

            var transaction = await this.dbContext.Transactions
                .FirstAsync(t => t.Hash == transactionHash);

            var accountAddress = transaction.Recipient == activity.CounterpartyAddress
                ? transaction.Sender
                : transaction.Recipient;

            var account = await this.dbContext.Accounts
                .FirstAsync(a => a.Address == accountAddress);

            if (account.BlockedBalance >= activity.BlockedAmount)
            {
                account.BlockedBalance -= activity.BlockedAmount;
                account.Balance += activity.BlockedAmount;

                activity.BlockedAmount = 0;
            }

            await this.dbContext.SaveChangesAsync();
        }

        private async Task<Activity> GetActivity(string transactionHash)
        {
            var activity = await this.dbContext.Activities
                .FirstOrDefaultAsync(a => a.TransactionHash == transactionHash);
            return activity;
        }
    }
}
