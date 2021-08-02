namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.Activities;

    public interface IActivityService
    {
        Task<bool> Exists(string transactionHash);

        Task AddActivity(ActivityServiceModel model);

        Task SetActivityStatus(string transactionHash, ActivityStatus status);

        Task<T> GetActivity<T>(string id);

        Task<IEnumerable<T>> GetLastActivities<T>(int count);

        Task<IEnumerable<T>> GetActivities<T>(int page, int pageSize);

        Task ReturnBlockedAmount(string transactionHash);
    }
}
