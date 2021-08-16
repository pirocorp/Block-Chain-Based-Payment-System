namespace PaymentSystem.WalletApp.Web.Tests.ControllersTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Tests.Mocks;
    using PaymentSystem.WalletApp.Web.Controllers;
    using PaymentSystem.WalletApp.Web.Infrastructure;
    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;
    using PaymentSystem.WalletApp.Web.ViewModels.Accounts.Index;
    using PaymentSystem.WalletApp.Web.ViewModels.Accounts.Transactions;

    using Xunit;

    public class AccountsControllerTests
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;

        public AccountsControllerTests()
        {
            MapperHelpers.Load();

            this.userManager = UserManagerMock.Get(new List<ApplicationUser>());
            this.dbContext = ApplicationDbContextMocks.Instance;
            this.accountService = new AccountService(this.dbContext, BlockChainGrpcServiceMock.Instance);
            this.transactionService = new TransactionService(this.dbContext, BlockChainGrpcServiceMock.Instance);
        }

        [Fact]
        public async Task IndexReturnsCorrectUserAccounts()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "piroman";

            this.dbContext.Users.Add(new ApplicationUser()
            {
                Id = userId,
                UserName = userName,
                Accounts = new List<Account>()
                {
                    new Account()
                    {
                        Address = Guid.NewGuid().ToString(),
                        UserId = userId,
                    },
                    new Account()
                    {
                        Address = Guid.NewGuid().ToString(),
                        UserId = userId,
                    },
                    new Account()
                    {
                        Address = Guid.NewGuid().ToString(),
                        UserId = userId,
                    },
                },
            });

            this.dbContext.Accounts.Add(new Account()
            {
                Address = Guid.NewGuid().ToString(),
                UserId = "Unique user",
            });

            await this.dbContext.SaveChangesAsync();

            var accountsController = new AccountsController(
                this.userManager,
                this.accountService,
                this.transactionService)
            {
                ControllerContext = ControllerContextMocks.LoggedInUser(userId, userName),
            };

            var result = await accountsController.Index();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AccountsIndexVewModel>(viewResult.Model);
            Assert.Equal(3, model.Accounts.Count());
        }

        [Fact]
        public async Task TransactionForAddressIsMappedCorrectly()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "piroman";

            this.dbContext.Users.Add(new ApplicationUser()
            {
                UserName = userName,
                Id = userId,
            });

            var address = Guid.NewGuid().ToString();

            this.dbContext.Accounts.Add(new Account()
            {
                Address = address,
                UserId = userId,
            });

            var dateTime = new DateTime(2021, 8, 16);

            var transaction = new Transaction()
            {
                Hash = Guid.NewGuid().ToString(),
                Amount = 5.55,
                Fee = 0.25,
                BlockHash = Guid.NewGuid().ToString(),
                Recipient = address,
                Sender = Guid.NewGuid().ToString(),
                TimeStamp = dateTime.Ticks,
            };

            this.dbContext.Transactions.Add(transaction);
            await this.dbContext.SaveChangesAsync();

            var accountsController = new AccountsController(
                this.userManager,
                this.accountService,
                this.transactionService)
            {
                ControllerContext = ControllerContextMocks.LoggedInUser(userId, userName),
            };

            var result = await accountsController.Transactions(address);

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AccountsTransactionsViewModel>(viewResult.Model);

            Assert.Equal(1, model.TotalPages);
            Assert.Equal(1, model.CurrentPage);
            Assert.Equal(address, model.CurrentAccountAddress);
            Assert.Single(model.Transactions);

            var currentTransaction = model.Transactions.First();
            Assert.Equal(transaction.Hash, currentTransaction.Hash);
            Assert.Equal(transaction.Sender, currentTransaction.Sender);
            Assert.Equal(transaction.Recipient, currentTransaction.Recipient);
            Assert.Equal(transaction.Amount, currentTransaction.Amount);
            Assert.Equal(transaction.Fee, currentTransaction.Fee);
            Assert.Equal(address, currentTransaction.CurrentAccountAddress);
            Assert.Equal(AddressHelpers.FriendlyAddress(transaction.Sender), currentTransaction.CounterpartyAddress);
            Assert.Equal(dateTime, currentTransaction.Date);
            Assert.NotEmpty(currentTransaction.Description);
            Assert.True(currentTransaction.Total > 0);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(15, 2)]
        [InlineData(20, 1)]
        [InlineData(50, 1)]
        [InlineData(100, 2)]
        public async Task TransactionsReturnsCorrectTransactionsForAddress(int count, int page)
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "piroman";

            this.dbContext.Users.Add(new ApplicationUser()
            {
                UserName = userName,
                Id = userId,
            });

            var address = Guid.NewGuid().ToString();

            this.dbContext.Accounts.Add(new Account()
            {
                Address = address,
                UserId = userId,
            });

            for (var i = 0; i < count; i++)
            {
                Transaction transaction = null;

                if (i % 2 == 0)
                {
                    transaction = new Transaction()
                    {
                        Recipient = address,
                    };
                }
                else
                {
                    transaction = new Transaction()
                    {
                        Sender = address,
                    };
                }

                transaction.Hash = Guid.NewGuid().ToString();

                this.dbContext.Transactions.Add(transaction);
                this.dbContext.Transactions.Add(new Transaction()
                {
                    Hash = Guid.NewGuid().ToString(),
                });
            }

            await this.dbContext.SaveChangesAsync();

            var accountsController = new AccountsController(
                this.userManager,
                this.accountService,
                this.transactionService)
            {
                ControllerContext = ControllerContextMocks.LoggedInUser(userId, userName),
            };

            var result = await accountsController.Transactions(address, page);
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AccountsTransactionsViewModel>(viewResult.Model);

            var totalPages = (int) Math.Ceiling(count / (double) WebConstants.DefaultTransactionsResultPageSize);
            var currentPage = Math.Max(1, page);

            var leftTransactions = count - (WebConstants.DefaultTransactionsResultPageSize * (currentPage - 1));
            var expectedTransactions = Math.Min(WebConstants.DefaultTransactionsResultPageSize, Math.Max(0, leftTransactions));

            Assert.Equal(totalPages, model.TotalPages);
            Assert.Equal(currentPage, model.CurrentPage);
            Assert.Equal(address, model.CurrentAccountAddress);
            Assert.Equal(expectedTransactions, model.Transactions.Count());
        }

        [Fact]
        public async Task TransactionsReturnsRedirectIfAddressIsNotBelongingToUser()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "piroman";

            this.dbContext.Users.Add(new ApplicationUser()
            {
                UserName = userName,
                Id = userId,
            });

            var address = Guid.NewGuid().ToString();

            this.dbContext.Accounts.Add(new Account()
            {
                Address = address,
                UserId = Guid.NewGuid().ToString(),
            });

            var accountsController = new AccountsController(
                this.userManager,
                this.accountService,
                this.transactionService)
            {
                ControllerContext = ControllerContextMocks.LoggedInUser(userId, userName),
            };

            var result = await accountsController.Transactions(address);
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.NotEmpty(redirectResult.ActionName);
        }
    }
}
