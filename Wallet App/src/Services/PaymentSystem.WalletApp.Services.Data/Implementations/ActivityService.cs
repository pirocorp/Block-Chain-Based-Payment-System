﻿namespace PaymentSystem.WalletApp.Services.Data.Implementations
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
            var activity = this.mapper.Map<Activity>(model);
            activity.TimeStamp = DateTime.UtcNow.Ticks;

            await this.dbContext.AddAsync(activity);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task SetActivityStatus(string transactionHash, ActivityStatus status)
        {
            var activity = await this.GetActivity(transactionHash);

            activity.Status = status;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<T> GetActivity<T>(string id)
            => await this.dbContext.Activities
                .Where(a => a.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task<(int Total, IEnumerable<T> Activities)> GetUserActivities<T>(
            string userId,
            int page,
            int pageSize,
            string dateRange = "")
        {
            var query = this.dbContext.Activities
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

                query = query
                    .Where(a => a.TimeStamp >= startTimeStamp && a.TimeStamp < endTimeStamp);
            }

            var total = await query.CountAsync();

            var activities = await query
                .OrderByDescending(a => a.TimeStamp)
                .To<T>()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, activities);
        }

        public async Task ReturnBlockedAmount(string transactionHash)
        {
            var activity = await this.dbContext.Activities
                .FirstOrDefaultAsync(a => a.TransactionHash == transactionHash);

            var account = await this.dbContext.Accounts
                .FirstOrDefaultAsync(a => a.UserId == activity.UserId);

            account.BlockedBalance -= activity.BlockedAmount;
            account.Balance += activity.BlockedAmount;

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
