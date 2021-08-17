namespace PaymentSystem.WalletApp.Web.Tests.ControllersTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Tests.Mocks;
    using PaymentSystem.WalletApp.Web.Controllers;
    using PaymentSystem.WalletApp.Web.ViewModels.Transactions;
    using PaymentSystem.WalletApp.Web.ViewModels.Transactions.SendCoins;
    using PaymentSystem.WalletApp.Web.ViewModels.Transactions.SendCoinsConfirm;

    using Xunit;

    public class TransactionsControllerTests
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;

        private readonly IAccountService accountService;
        private readonly IActivityService activityService;
        private readonly IAccountsKeyService accountsKeyService;
        private readonly IMapper mapper;
        private readonly ITransactionService transactionService;
        private readonly IUserService userService;
        private readonly TransactionsController transactionsController;

        private readonly string userId;
        private readonly string username;

        public TransactionsControllerTests()
        {
            MapperHelpers.Load();

            this.userManager = UserManagerMock.Get(new List<ApplicationUser>());
            this.dbContext = ApplicationDbContextMocks.Instance;

            this.accountService = new AccountService(this.dbContext, BlockChainGrpcServiceMock.Instance);
            this.accountsKeyService = AccountsKeyServiceMock.Instance;
            this.mapper = MapperHelpers.Instance;
            this.transactionService = new TransactionService(this.dbContext, BlockChainGrpcServiceMock.Instance);
            this.activityService = new ActivityService(this.dbContext, this.mapper);

            this.userService = UserServiceMock.Instance;

            this.transactionsController = new TransactionsController(
                this.accountService,
                this.accountsKeyService,
                this.mapper,
                this.transactionService,
                this.userManager,
                this.userService);

            this.userId = Guid.NewGuid().ToString();
            this.username = "piroman";
        }

        [Fact]
        public async Task GetTransactionDetailsReturnsCorrectTransactionDetails()
        {
            var date = DateTime.Now;

            var transaction = new Transaction()
            {
                Hash = Guid.NewGuid().ToString(),
                Sender = Guid.NewGuid().ToString(),
                Recipient = Guid.NewGuid().ToString(),
                Amount = 15.05,
                BlockHash = Guid.NewGuid().ToString(),
                Fee = 5.12,
                TimeStamp = date.Ticks,
            };

            this.dbContext.Transactions.Add(transaction);
            await this.dbContext.SaveChangesAsync();

            var response = await this.transactionsController.GetTransactionDetails(transaction.Hash);

            Assert.NotNull(response);
            var objectResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            var transactionDetails = Assert.IsType<TransactionDetails>(objectResult.Value);

            Assert.Equal(transaction.Hash, transactionDetails.Hash);
            Assert.Equal(transaction.Sender, transactionDetails.Sender);
            Assert.Equal(transaction.Recipient, transactionDetails.Recipient);
            Assert.Equal(transaction.BlockHash, transactionDetails.BlockHash);
            Assert.Equal(transaction.Amount, transactionDetails.Amount);
            Assert.Equal(transaction.Fee, transactionDetails.Fee);
            Assert.Equal(transaction.TimeStamp, transactionDetails.TimeStamp);
            Assert.Equal(date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture), transactionDetails.Date);
        }

        [Fact]
        public async Task GetTransactionDetailsReturnsEmptyObjectIfTransactionIsNotFound()
        {
            var response = await this.transactionsController.GetTransactionDetails(Guid.NewGuid().ToString());

            Assert.NotNull(response);
            var objectResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.Null(objectResult.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public async Task SendCoinsReturnsUsersAccountsToChooseFrom(int count)
        {
            this.SeedLoggedUser();
            this.SeedAccounts(count);

            var result = await this.transactionsController.SendCoins();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SendCoinsViewModel>(viewResult.Model);

            Assert.Equal(count, model.Accounts.Count());
        }

        [Fact]
        public async Task SendCoinsReturnsCorrectAccountsRepresentations()
        {
            this.SeedLoggedUser();

            var storedKeyAccount = Guid.NewGuid().ToString();
            var account = new Account()
            {
                Address = storedKeyAccount,
                Balance = 100.50,
                UserId = this.userId,
                AccountKey = new AccountKey()
                {
                    UserId = this.userId,
                    Address = storedKeyAccount,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Key = Guid.NewGuid().ToString(),
                },
            };

            this.dbContext.Accounts.Add(account);

            await this.dbContext.SaveChangesAsync();

            var result = await this.transactionsController.SendCoins();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SendCoinsViewModel>(viewResult.Model);

            Assert.Single(model.Accounts);
            var coinAccount = model.Accounts.First();

            Assert.Equal(account.Address, coinAccount.Address);
            Assert.Equal(account.Balance, coinAccount.Balance);
            Assert.True(coinAccount.HasStoredKey);
        }

        [Fact]
        public async Task SendCoinPostIfUserDoNotOwnAccountRedirects()
        {
            this.SeedLoggedUser();
            this.SeedAccounts(3);

            var model = new SendCoinInputModel()
            {
                CoinAccount = Guid.NewGuid().ToString(),
                Recipient = Guid.NewGuid().ToString(),
                Amount = 50,
            };

            var result = await this.transactionsController.SendCoins(model);

            Assert.NotNull(result);
            Assert.False(this.transactionsController.ModelState.IsValid);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task SendCoinPostIfUserHasNotSufficientFundsRedirects()
        {
            this.SeedLoggedUser();
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = this.userId,
                Balance = 200,
            };

            this.dbContext.Add(account);
            await this.dbContext.SaveChangesAsync();

            var model = new SendCoinInputModel()
            {
                CoinAccount = account.Address,
                Recipient = Guid.NewGuid().ToString(),
                Amount = 500,
            };

            var result = await this.transactionsController.SendCoins(model);

            Assert.NotNull(result);
            Assert.False(this.transactionsController.ModelState.IsValid);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task SendCoinPostValidModelRedirectsToSendCoinConfirm()
        {
            this.SeedLoggedUser();
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = this.userId,
                Balance = 500,
            };

            this.dbContext.Add(account);
            await this.dbContext.SaveChangesAsync();

            var model = new SendCoinInputModel()
            {
                CoinAccount = account.Address,
                Recipient = Guid.NewGuid().ToString(),
                Amount = 150,
            };

            this.transactionsController.TempData = TempDataMock.Instance;

            var result = await this.transactionsController.SendCoins(model);

            Assert.NotNull(result);
            Assert.True(this.transactionsController.ModelState.IsValid);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SendCoinsConfirm", redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
            Assert.Equal(1, this.transactionsController.TempData.Count);
            Assert.Equal(JsonConvert.SerializeObject(model), this.transactionsController.TempData["Send Coin Temp Data"]);
        }

        [Fact]
        public void SendCoinConfirmRedirectIfTempDataDoesNotContainModel()
        {
            this.transactionsController.TempData = TempDataMock.Instance;

            var result = this.transactionsController.SendCoinsConfirm();

            Assert.NotNull(result);
            Assert.True(this.transactionsController.ModelState.IsValid);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public void SendCoinConfirmShowsViewIfTempDataContainsModel()
        {
            var model = new SendCoinInputModel()
            {
                CoinAccount = Guid.NewGuid().ToString(),
                Recipient = Guid.NewGuid().ToString(),
                Amount = 150,
            };

            this.transactionsController.TempData = TempDataMock.Instance;
            this.transactionsController.TempData["Send Coin Temp Data"] = JsonConvert.SerializeObject(model);

            var result = this.transactionsController.SendCoinsConfirm();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<SendCoinsConfirmViewModel>(viewResult.Model);

            var fees = model.Amount * (WalletConstants.DefaultFeePercent / 100);

            Assert.Equal(model.CoinAccount, viewModel.CoinAccount);
            Assert.Equal(model.Recipient, viewModel.Recipient);
            Assert.Equal(model.HasKey, viewModel.HasKey);
            Assert.Equal(fees, viewModel.Fees);
            Assert.Equal(model.Amount, viewModel.Amount);
            Assert.Equal(model.Amount + fees, viewModel.TotalAmount);
        }

        [Fact]
        public async Task SendCoinsConfirmPostIfUserDoNotOwnsAccountRedirects()
        {
            this.SeedLoggedUser();
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Balance = 500,
            };

            this.dbContext.Add(account);
            await this.dbContext.SaveChangesAsync();

            var model = new SendCoinConfirmInputModel()
            {
                CoinAccount = account.Address,
                Recipient = Guid.NewGuid().ToString(),
                Amount = 150,
                Secret = Guid.NewGuid().ToString(),
            };

            var result = await this.transactionsController.SendCoinsConfirm(model);

            Assert.NotNull(result);
            Assert.False(this.transactionsController.ModelState.IsValid);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task SendCoinsConfirmPostIfUserDoNotHaveSufficientFundsRedirects()
        {
            this.SeedLoggedUser();
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = this.userId,
                Balance = 150,
            };

            this.dbContext.Add(account);
            await this.dbContext.SaveChangesAsync();

            var model = new SendCoinConfirmInputModel()
            {
                CoinAccount = account.Address,
                Recipient = Guid.NewGuid().ToString(),
                Amount = 500,
                Secret = Guid.NewGuid().ToString(),
            };

            var result = await this.transactionsController.SendCoinsConfirm(model);

            Assert.NotNull(result);
            Assert.False(this.transactionsController.ModelState.IsValid);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task SendCoinsConfirmPostIfSecretCanNotBeFoundFundsRedirects()
        {
            this.SeedLoggedUser();
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = this.userId,
                Balance = 150,
            };

            this.dbContext.Add(account);
            await this.dbContext.SaveChangesAsync();

            var model = new SendCoinConfirmInputModel()
            {
                CoinAccount = account.Address,
                Recipient = Guid.NewGuid().ToString(),
                Amount = 500,
            };

            var result = await this.transactionsController.SendCoinsConfirm(model);

            Assert.NotNull(result);
            Assert.False(this.transactionsController.ModelState.IsValid);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task SendCoinsConfirmPostIfSendCoinFailsRedirects()
        {
            this.SeedLoggedUser();
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = this.userId,
                Balance = 150,
            };

            this.dbContext.Add(account);
            await this.dbContext.SaveChangesAsync();

            var model = new SendCoinConfirmInputModel()
            {
                CoinAccount = account.Address,
                Recipient = Guid.NewGuid().ToString(),
                Secret = Guid.NewGuid().ToString(),
                Amount = -5,
            };

            var result = await this.transactionsController.SendCoinsConfirm(model);

            Assert.NotNull(result);
            Assert.False(this.transactionsController.ModelState.IsValid);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task SendCoinsConfirmPostRedirectToSendCoinsSuccess()
        {
            this.SeedLoggedUser();
            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = this.userId,
                Balance = 500,
            };

            this.dbContext.Add(account);
            await this.dbContext.SaveChangesAsync();

            var model = new SendCoinConfirmInputModel()
            {
                CoinAccount = account.Address,
                Recipient = Guid.NewGuid().ToString(),
                Secret = Guid.NewGuid().ToString(),
                Amount = 150,
            };

            var result = await this.transactionsController.SendCoinsConfirm(model);

            Assert.NotNull(result);
            Assert.True(this.transactionsController.ModelState.IsValid);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SendCoinsSuccess", redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public void SendCoinsSuccessReturnsView()
        {
            var result = this.transactionsController.SendCoinsSuccess();

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        private void SeedLoggedUser()
        {
            this.dbContext.Add(new ApplicationUser()
            {
                UserName = this.username,
                Id = this.userId,
            });

            this.dbContext.SaveChanges();

            this.transactionsController.ControllerContext =
                ControllerContextMocks.LoggedInUser(this.userId, this.username);
        }

        private void SeedAccounts(int count)
        {
            // Add user's accounts
            this.dbContext.AddRange(Enumerable.Range(1, count)
                .Select(i => new Account()
                {
                    Address = Guid.NewGuid().ToString(),
                    UserId = this.userId,
                }));

            // Add random accounts
            this.dbContext.AddRange(Enumerable.Range(1, count)
                .Select(i => new Account()
                {
                    Address = Guid.NewGuid().ToString(),
                    UserId = $"Random user {i}",
                }));

            this.dbContext.SaveChanges();
        }
    }
}
