namespace PaymentSystem.WalletApp.Web.Tests.ControllersTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Tests.Mocks;
    using PaymentSystem.WalletApp.Web.Controllers;
    using PaymentSystem.WalletApp.Web.Infrastructure;
    using PaymentSystem.WalletApp.Web.ViewModels.Activities.Index;

    using Xunit;

    public class ActivitiesControllerTests
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IActivityService activityService;
        private readonly ActivitiesController activitiesController;

        private readonly string userId;
        private readonly string userName;

        public ActivitiesControllerTests()
        {
            MapperHelpers.Load();

            this.userManager = UserManagerMock.Get(new List<ApplicationUser>());
            this.dbContext = ApplicationDbContextMocks.Instance;

            this.activityService = new ActivityService(this.dbContext, MapperHelpers.Instance);

            this.activitiesController = new ActivitiesController(this.userManager, this.activityService);

            this.userId = Guid.NewGuid().ToString();
            this.userName = "piroman";
        }

        [Theory]
        [InlineData(15, 1)]
        [InlineData(100, 1)]
        [InlineData(28, 3)]
        [InlineData(5, 2)]
        [InlineData(0, 1)]
        [InlineData(15, -1)]
        public async Task IndexShouldReturnCorrectActivities(int count, int page)
        {
            await this.InitializeDatabaseWithUserAndActivities(count, "");

            this.activitiesController.ControllerContext = ControllerContextMocks
                .LoggedInUser(this.userId, this.userName);

            var result = await this.activitiesController.Index(page);

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ActivitiesIndexViewModel>(viewResult.Model);

            var (totalPages, currentPage, expectedItems) = CalculateTotalCurrentPageAndExpectedItems(count, page);

            Assert.Equal(totalPages, model.TotalPages);
            Assert.Equal(currentPage, model.CurrentPage);
            Assert.Equal(expectedItems, model.Activities.Count());
        }

        [Theory]
        [InlineData(15, 1, "07/19/2021 - 08/17/2021")]
        [InlineData(15, 2, "07/19/2021 - 08/17/2021")]
        public async Task IndexShouldFilterCorrectResultsBasedOnDates(int count, int page, string dateRange)
        {
            await this.InitializeDatabaseWithUserAndActivities(count, dateRange);

            this.activitiesController.ControllerContext = ControllerContextMocks
                .LoggedInUser(this.userId, this.userName);

            var result = await this.activitiesController.Index(page, dateRange);

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ActivitiesIndexViewModel>(viewResult.Model);

            var (totalPages, currentPage, expectedItems) = CalculateTotalCurrentPageAndExpectedItems(count, page);

            Assert.Equal(totalPages, model.TotalPages);
            Assert.Equal(currentPage, model.CurrentPage);
            Assert.Equal(expectedItems, model.Activities.Count());
        }

        [Fact]
        public async Task IndexMapActivityCorrectly()
        {
            this.dbContext.Users.Add(new ApplicationUser()
            {
                UserName = this.userName,
                Id = this.userId,
            });

            var date = DateTime.Now;

            var activity = new Activity()
            {
                UserId = this.userId,
                Amount = 150.05,
                BlockedAmount = 155.25,
                CounterpartyAddress = Guid.NewGuid().ToString(),
                Description = "Description",
                Status = ActivityStatus.Pending,
                TimeStamp = date.Ticks,
                TransactionHash = Guid.NewGuid().ToString(),
            };

            this.dbContext.Activities.Add(activity);
            await this.dbContext.SaveChangesAsync();

            this.activitiesController.ControllerContext = ControllerContextMocks
                .LoggedInUser(this.userId, this.userName);

            var result = await this.activitiesController.Index();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ActivitiesIndexViewModel>(viewResult.Model);
            Assert.Equal(1, model.TotalPages);
            Assert.Equal(1, model.CurrentPage);
            Assert.Single(model.Activities);

            var actualActivity = model.Activities.First();

            Assert.Equal(activity.Amount, actualActivity.Amount);
            Assert.Equal(activity.CounterpartyAddress, actualActivity.CounterpartyAddress);
            Assert.Equal(activity.Description, actualActivity.Description);
            Assert.Equal(activity.Status, actualActivity.Status);
            Assert.Equal(activity.TimeStamp, actualActivity.TimeStamp);
            Assert.Equal(activity.TransactionHash, actualActivity.TransactionHash);
            Assert.Equal(date, actualActivity.Date);
        }

        private static (int totalPages, int currentPage, int expectedItems)
            CalculateTotalCurrentPageAndExpectedItems(int count, int page)
        {
            var totalPages = (int)Math.Ceiling(count / (double)WebConstants.DefaultActivitiesResultPageSize);
            var currentPage = Math.Max(1, page);

            var leftActivities = count - (WebConstants.DefaultActivitiesResultPageSize * (currentPage - 1));
            var expectedTransactions = Math.Min(WebConstants.DefaultActivitiesResultPageSize, Math.Max(0, leftActivities));
            return (totalPages, currentPage, expectedTransactions);
        }

        private async Task InitializeDatabaseWithUserAndActivities(int count, string dateRange)
        {
            var hasDate = !string.IsNullOrWhiteSpace(dateRange);

            DateTime startDate = default;
            DateTime endDate = default;

            if (hasDate)
            {
                var dates = dateRange.Split("-").Select(d => d.Trim()).ToList();
                startDate = DateTime.Parse(dates[0], CultureInfo.InvariantCulture);
                endDate = DateTime.Parse(dates[1], CultureInfo.InvariantCulture);
            }

            this.dbContext.Users.Add(new ApplicationUser()
            {
                UserName = this.userName,
                Id = this.userId,
            });

            // Add user's activities.
            await this.dbContext.AddRangeAsync(Enumerable.Range(1, count)
                .Select(i => new Activity()
                {
                    UserId = this.userId,
                    CounterpartyAddress = Guid.NewGuid().ToString(),
                    TransactionHash = Guid.NewGuid().ToString(),
                    TimeStamp = hasDate ? startDate.AddHours(i).Ticks : DateTime.Now.Ticks,
                }));

            // Add filtered activities.
            await this.dbContext.AddRangeAsync(Enumerable.Range(1, count)
                .Select(i => new Activity()
                {
                    UserId = hasDate ? this.userId : Guid.NewGuid().ToString(),
                    CounterpartyAddress = Guid.NewGuid().ToString(),
                    TransactionHash = Guid.NewGuid().ToString(),
                    TimeStamp = hasDate ? endDate.AddDays(1).AddMinutes(i).Ticks : DateTime.Now.Ticks,
                }));

            await this.dbContext.SaveChangesAsync();
        }
    }
}
