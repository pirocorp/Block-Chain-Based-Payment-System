namespace PaymentSystem.WalletApp.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Services.Data.Models.Activities;
    using PaymentSystem.WalletApp.Tests.Mocks;

    using Xunit;

    public class ActivityServiceTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        private readonly IActivityService activityService;

        public ActivityServiceTests()
        {
            MapperHelpers.Load<Activity, ActivityDerivative>();

            this.dbContext = ApplicationDbContextMocks.Instance;
            this.mapper = MapperHelpers.Instance;

            this.activityService = new ActivityService(this.dbContext, this.mapper);
        }

        [Fact]
        public async Task ExistsReturnsTrueWhenActivityExists()
        {
            var activity = new Activity()
            {
                TransactionHash = Guid.NewGuid().ToString(),
            };

            this.dbContext.Add(activity);
            await this.dbContext.SaveChangesAsync();

            Assert.True(await this.activityService.Exists(activity.TransactionHash));
        }

        [Fact]
        public async Task ExistsReturnsFalseWhenActivityDoNotExists()
        {
            var activity = new Activity()
            {
                TransactionHash = Guid.NewGuid().ToString(),
            };

            this.dbContext.Add(activity);
            await this.dbContext.SaveChangesAsync();

            Assert.False(await this.activityService.Exists(Guid.NewGuid().ToString()));
        }

        [Fact]
        public async Task AddActivityHandlesNullWithoutPerformingAnything()
        {
            await this.activityService.AddActivity(null);
        }

        [Fact]
        public async Task AddActivityAddsActivityToDatabase()
        {
            var model = new ActivityServiceModel()
            {
                Amount = 5.50,
                BlockedAmount = 1.20,
                CounterpartyAddress = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Status = ActivityStatus.Pending,
                TransactionHash = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
            };

            await this.activityService.AddActivity(model);

            var actual = this.dbContext.Activities
                .First(a => a.TransactionHash == model.TransactionHash);

            Assert.Equal(model.Amount, actual.Amount);
            Assert.Equal(model.BlockedAmount, actual.BlockedAmount);
            Assert.Equal(model.CounterpartyAddress, actual.CounterpartyAddress);
            Assert.Equal(model.Description, actual.Description);
            Assert.Equal(model.Status, actual.Status);
            Assert.Equal(model.TransactionHash, actual.TransactionHash);
            Assert.Equal(model.UserId, actual.UserId);
        }

        [Fact]
        public async Task SetActivityStatusDoNothingIfActivityIsNotFound()
        {
            await this.activityService.SetActivityStatus(Guid.NewGuid().ToString(), ActivityStatus.Canceled);
        }

        [Fact]
        public async Task SetActivityStatusUpdatesStatusCorrectly()
        {
            var activity = new Activity()
            {
                TransactionHash = Guid.NewGuid().ToString(),
                Status = ActivityStatus.Completed,
            };

            this.dbContext.Add(activity);
            await this.dbContext.SaveChangesAsync();

            await this.activityService.SetActivityStatus(activity.TransactionHash, ActivityStatus.Canceled);

            Assert.Equal(ActivityStatus.Canceled, activity.Status);
        }

        [Fact]
        public async Task GetActivityReturnsCorrectActivity()
        {
            var activity = new Activity()
            {
                TransactionHash = Guid.NewGuid().ToString(),
                Status = ActivityStatus.Completed,
            };

            this.dbContext.Add(activity);
            await this.dbContext.SaveChangesAsync();

            var derivative = await this.activityService
                .GetActivity<ActivityDerivative>(activity.Id);

            Assert.Equal(activity.TransactionHash, derivative.TransactionHash);
        }

        [Fact]
        public async Task ReturnBlockedAmountHandlesStringEmpty()
        {
            await this.activityService.ReturnBlockedAmount(null);
            await this.activityService.ReturnBlockedAmount(string.Empty);
            await this.activityService.ReturnBlockedAmount("    ");
        }

        [Fact]
        public async Task ReturnBlockedAmountRestoreBalanceWithBlockedAmount()
        {
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Balance = 500,
                BlockedBalance = 350,
            };

            var transaction = new Transaction()
            {
                Hash = Guid.NewGuid().ToString(),
                Sender = account.Address,
                Recipient = Guid.NewGuid().ToString(),
            };

            var activity = new Activity()
            {
                TransactionHash = transaction.Hash,
                CounterpartyAddress = transaction.Recipient,
                BlockedAmount = 150,
            };

            this.dbContext.Add(account);
            this.dbContext.Add(transaction);
            this.dbContext.Add(activity);
            await this.dbContext.SaveChangesAsync();

            await this.activityService.ReturnBlockedAmount(transaction.Hash);

            Assert.Equal(650, account.Balance);
            Assert.Equal(200, account.BlockedBalance);
            Assert.Equal(0, activity.BlockedAmount);
        }

        private class ActivityDerivative
        {
            public string TransactionHash { get; set; }
        }
    }
}
