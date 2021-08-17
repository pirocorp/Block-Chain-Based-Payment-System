namespace PaymentSystem.WalletApp.Web.Tests.ControllersTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using PaymentSystem.Common.Utilities;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Tests.Mocks;
    using PaymentSystem.WalletApp.Web.Controllers;
    using Services.Data.Models;
    using Services.Implementations;
    using ViewModels.Transfers.Deposit;
    using ViewModels.Transfers.DepositConfirm;
    using Xunit;

    public class TransfersControllerTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly IAccountService accountService;
        private readonly IActivityService activityService;
        private readonly IBankAccountService bankAccountService;
        private readonly ICreditCardService creditCardService;
        private readonly ITransferService transferService;
        private readonly ITransactionService transactionService;
        private readonly IUserService userService;

        private readonly TransfersController transfersController;

        private readonly string userId;
        private readonly string username;

        public TransfersControllerTests()
        {
            MapperHelpers.Load();

            this.dbContext = ApplicationDbContextMocks.Instance;
            this.userManager = UserManagerMock.Get(new List<ApplicationUser>());
            this.mapper = MapperHelpers.Instance;
            this.accountService = new AccountService(this.dbContext, BlockChainGrpcServiceMock.Instance);
            this.activityService = new ActivityService(this.dbContext, this.mapper);
            this.bankAccountService = new BankAccountService(this.mapper, this.dbContext);

            this.creditCardService = new CreditCardService(
                this.dbContext,
                SaltServiceMock.Instance,
                new SecurelyEncryptDataService(),
                FingerprintServiceMock.Instance,
                this.mapper);

            this.transferService = TransferServiceMock.Instance;

            this.transactionService = new TransactionService(this.dbContext, BlockChainGrpcServiceMock.Instance);

            this.userService = new UserService(
                this.dbContext,
                this.accountService,
                AccountsKeyServiceMock.Instance,
                this.activityService,
                this.transactionService);

            this.transfersController = new TransfersController(
                this.userManager,
                this.mapper,
                this.accountService,
                this.bankAccountService,
                this.creditCardService,
                this.transferService,
                this.userService);

            this.userId = Guid.NewGuid().ToString();
            this.username = "piroman";
        }

        [Fact]
        public void DepositReturnsViewResult()
        {
            var result = this.transfersController.Deposit();

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void DepositPostReturnsRedirectWithInvalidModelState()
        {
            this.transfersController.ModelState.AddModelError(string.Empty, "Model Error");

            var result = this.transfersController.Deposit(new DepositModel());

            Assert.NotNull(result);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public void DepositPostRedirectsToDepositConfirmAndStoreModelToTempData()
        {
            var model = new DepositModel()
            {
                Amount = 5.55,
                PaymentMethod = PaymentMethod.BankAccount,
            };

            this.transfersController.TempData = TempDataMock.Instance;
            var result = this.transfersController.Deposit(model);

            Assert.Equal(JsonConvert.SerializeObject(model), this.transfersController.TempData["depositTempData"]);

            Assert.NotNull(result);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DepositConfirm", redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task DepositConfirmReturnsRedirectIfNoTempDataPresent()
        {
            this.transfersController.TempData = TempDataMock.Instance;
            var result = await this.transfersController.DepositConfirm();

            Assert.NotNull(result);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Theory]
        [InlineData(3, 3)]
        [InlineData(4, 2)]
        [InlineData(1, 5)]
        [InlineData(2, 6)]
        public async Task DepositConfirmReturnsViewWithCorrectBankAccountsAndCoinAccounts<T>(int coinAccounts, int paymentMethods)
        {
            this.SeedLoggedUser();
            this.SeedCoinAccounts(coinAccounts);
            this.SeedBankAccounts(paymentMethods);

            var model = new DepositModel()
            {
                Amount = 5.55,
                PaymentMethod = PaymentMethod.BankAccount,
            };

            this.transfersController.TempData = TempDataMock.Instance;
            this.transfersController.TempData["depositTempData"] = JsonConvert.SerializeObject(model);

            var result = await this.transfersController.DepositConfirm();

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<DepositConfirmModel>(viewResult.Model);

            Assert.Equal(paymentMethods, viewModel.PaymentMethods.Count());
            Assert.Equal(coinAccounts, viewModel.Accounts.Count());
        }

        [Theory]
        [InlineData(3, 3)]
        [InlineData(4, 2)]
        [InlineData(1, 5)]
        [InlineData(2, 6)]
        public async Task DepositConfirmReturnsViewWithCorrectCreditCardAccountsAndCoinAccounts<T>(int coinAccounts, int paymentMethods)
        {
            this.SeedLoggedUser();
            this.SeedCoinAccounts(coinAccounts);
            this.SeedCreditCards(paymentMethods);

            var model = new DepositModel()
            {
                Amount = 5.55,
                PaymentMethod = PaymentMethod.CreditOrDebitCard,
            };

            this.transfersController.TempData = TempDataMock.Instance;
            this.transfersController.TempData["depositTempData"] = JsonConvert.SerializeObject(model);

            var result = await this.transfersController.DepositConfirm();

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<DepositConfirmModel>(viewResult.Model);

            Assert.Equal(paymentMethods, viewModel.PaymentMethods.Count());
            Assert.Equal(coinAccounts, viewModel.Accounts.Count());
        }

        [Fact]
        public async Task DepositConfirmPostReturnsRedirectWithInvalidModelState()
        {
            this.transfersController.ModelState.AddModelError(string.Empty, "Model Error");

            var result = await this.transfersController.DepositConfirm(new DepositConfirmInputModel());

            Assert.NotNull(result);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task DepositConfirmPostRedirectIfUserDoNotOwnAccount()
        {
            this.SeedLoggedUser();

            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
            };

            var bankAccount = new BankAccount()
            {
                IBAN = Guid.NewGuid().ToString(),
                UserId = this.userId,
            };

            this.dbContext.Add(bankAccount);
            this.dbContext.Add(account);

            await this.dbContext.SaveChangesAsync();

            var model = new DepositConfirmInputModel()
            {
                Account = account.Address,
                PaymentType = PaymentMethod.BankAccount,
                PaymentMethod = bankAccount.Id,
            };

            var result = await this.transfersController.DepositConfirm(model);

            Assert.False(this.transfersController.ModelState.IsValid);
            Assert.NotNull(result);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task DepositConfirmPostRedirectIfUserDoNotOwnPaymentMethod()
        {
            this.SeedLoggedUser();

            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = this.userId,
            };

            var bankAccount = new CreditCard()
            {
                CardData = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
            };

            this.dbContext.Add(bankAccount);
            this.dbContext.Add(account);

            await this.dbContext.SaveChangesAsync();

            var model = new DepositConfirmInputModel()
            {
                Account = account.Address,
                PaymentType = PaymentMethod.CreditOrDebitCard,
                PaymentMethod = bankAccount.Id,
            };

            var result = await this.transfersController.DepositConfirm(model);

            Assert.False(this.transfersController.ModelState.IsValid);
            Assert.NotNull(result);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task DepositConfirmPostRedirectIfDepositToAccountFails()
        {
            this.SeedLoggedUser();

            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = this.userId,
            };

            var credCard = new CreditCard()
            {
                CardData = Guid.NewGuid().ToString(),
                UserId = this.userId,
            };

            this.dbContext.Add(credCard);
            this.dbContext.Add(account);

            await this.dbContext.SaveChangesAsync();

            var model = new DepositConfirmInputModel()
            {
                Account = account.Address,
                PaymentType = PaymentMethod.CreditOrDebitCard,
                PaymentMethod = credCard.Id,
                Amount = -5,
            };

            var result = await this.transfersController.DepositConfirm(model);

            Assert.False(this.transfersController.ModelState.IsValid);
            Assert.NotNull(result);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public async Task DepositConfirmPostRedirectIfToDepositSuccessAndConfirmBankAccount()
        {
            this.SeedLoggedUser();

            var account = new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = this.userId,
            };

            var bankAccount = new BankAccount()
            {
                IBAN = Guid.NewGuid().ToString(),
                UserId = this.userId,
            };

            this.dbContext.Add(bankAccount);
            this.dbContext.Add(account);

            await this.dbContext.SaveChangesAsync();

            var model = new DepositConfirmInputModel()
            {
                Account = account.Address,
                PaymentType = PaymentMethod.BankAccount,
                PaymentMethod = bankAccount.Id,
                Amount = 50,
            };

            var result = await this.transfersController.DepositConfirm(model);

            Assert.True(this.transfersController.ModelState.IsValid);
            Assert.True(bankAccount.IsApproved);

            Assert.NotNull(result);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("DepositSuccess", redirectToAction.ActionName);
            Assert.False(redirectToAction.Permanent);
        }

        [Fact]
        public void DepositSuccessReturnsView()
        {
            var result = this.transfersController.DepositSuccess();

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

            this.transfersController.ControllerContext =
                ControllerContextMocks.LoggedInUser(this.userId, this.username);
        }

        private void SeedBankAccounts(int count)
        {
            // Add user's accounts
            this.dbContext.AddRange(Enumerable.Range(1, count)
                .Select(i => new BankAccount()
                {
                    IBAN = Guid.NewGuid().ToString(),
                    UserId = this.userId,
                }));

            // Add random accounts
            this.dbContext.AddRange(Enumerable.Range(1, count)
                .Select(i => new BankAccount()
                {
                    IBAN = Guid.NewGuid().ToString(),
                    UserId = $"Random user {i}",
                }));

            this.dbContext.SaveChanges();
        }

        private void SeedCoinAccounts(int count)
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

        private void SeedCreditCards(int count)
        {
            // Add user's accounts
            this.dbContext.AddRange(Enumerable.Range(1, count)
                .Select(i => new CreditCard()
                {
                    CardData = Guid.NewGuid().ToString(),
                    UserId = this.userId,
                }));

            // Add random accounts
            this.dbContext.AddRange(Enumerable.Range(1, count)
                .Select(i => new CreditCard()
                {
                    CardData = Guid.NewGuid().ToString(),
                    UserId = $"Random user {i}",
                }));

            this.dbContext.SaveChanges();
        }
    }
}
