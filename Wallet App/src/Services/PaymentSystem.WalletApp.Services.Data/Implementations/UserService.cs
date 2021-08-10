namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.Common.Transactions;
    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.Activities;
    using PaymentSystem.WalletApp.Services.Data.Models.BankAccounts;
    using PaymentSystem.WalletApp.Services.Data.Models.CreditCards;

    using static PaymentSystem.WalletApp.Web.Infrastructure.WebConstants;

    public class UserService : IUserService
    {
        private const int BankAccountLastDigitCount = 4;
        private const int CoinAccountLastDigitCount = 12;

        private readonly ApplicationDbContext dbContext;
        private readonly IAccountService accountService;
        private readonly IAccountsKeyService accountsKeyService;
        private readonly IActivityService activityService;
        private readonly ITransactionService transactionService;

        public UserService(
            ApplicationDbContext dbContext,
            IAccountService accountService,
            IAccountsKeyService accountsKeyService,
            IActivityService activityService,
            ITransactionService transactionService)
        {
            this.dbContext = dbContext;
            this.accountService = accountService;
            this.accountsKeyService = accountsKeyService;
            this.activityService = activityService;
            this.transactionService = transactionService;
        }

        public async Task<T> GetUser<T>(string id)
            => await this.dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<T>> GetBankAccounts<T>(string id)
            => await this.dbContext.BankAccounts
                .Where(a => a.UserId == id)
                .To<T>()
                .ToListAsync();

        public async Task<IEnumerable<T>> GetCreditCards<T>(string id)
            => await this.dbContext.CreditCards
                .Where(a => a.UserId == id)
                .To<T>()
                .ToListAsync();

        public async Task<IEnumerable<SelectListItem>> GetPaymentMethods(string userId, PaymentMethod paymentMethod)
            => paymentMethod switch
            {
                PaymentMethod.CreditOrDebitCard => (await this.GetCreditCards<CreditCardSelectListModel>(userId))
                    .Select(c => new SelectListItem($"XXXX - {c.LastFourDigits}", c.Id))
                    .ToList(),

                PaymentMethod.BankAccount => (await this.GetBankAccounts<BankAccountsSelectListModel>(userId))
                    .Select(b => new SelectListItem($"{b.BankName} - {b.IBAN[^BankAccountLastDigitCount..]}", b.Id))
                    .ToList(),

                _ => new List<SelectListItem>(),
            };

        public async Task<IEnumerable<SelectListItem>> GetCoinAccounts(string userId)
            => (await this.dbContext.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    a.Address,
                })
                .ToListAsync())
                .Select(a => new SelectListItem($"XXXX - {a.Address[^CoinAccountLastDigitCount..]}", a.Address))
                .ToList();

        public async Task<IEnumerable<T>> GetLatestRegisteredUsers<T>()
            => await this.dbContext.Users
                .OrderByDescending(u => u.CreatedOn)
                .To<T>()
                .Take(DefaultUsersResultPageSize)
                .ToListAsync();

        public async Task<bool> SendCoins(string senderAddress, string recipientAddress, double amount, string secret, string userId)
        {
            if (string.IsNullOrWhiteSpace(secret))
            {
                var keyData = await this.accountsKeyService.GetKeyData(senderAddress, userId);
                secret = keyData?.Secret;
            }

            var publicKey = await this.accountService.GetPublicKey(senderAddress);
            var fee = amount * (WalletConstants.DefaultFeePercent / 100);

            var (transactionStatus, transactionHash) = await this.transactionService.CreateTransaction(
                senderAddress,
                recipientAddress,
                amount,
                fee,
                secret,
                publicKey);

            var description = string.Format($"Send coins from {senderAddress[^12..]} to {recipientAddress[^12..]}");

            var success = TransactionIsSuccessful(transactionStatus);
            var blockedAmount = amount + fee;

            if (success)
            {
                await this.accountService.BlockFunds(senderAddress, blockedAmount);
            }

            var activity = new ActivityServiceModel
            {
                Amount = amount + fee,
                CounterpartyAddress = recipientAddress,
                Description = description,
                UserId = userId,
                Status = success ? ActivityStatus.Pending : ActivityStatus.Canceled,
                TransactionHash = transactionHash,
                BlockedAmount = blockedAmount,
            };

            await this.activityService.AddActivity(activity);

            return success;
        }

        private static bool TransactionIsSuccessful(TransactionStatus transactionStatus)
            => transactionStatus == TransactionStatus.Pending;
    }
}
