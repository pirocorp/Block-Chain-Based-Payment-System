namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.Activities;

    public interface IActivityService
    {
        Task<bool> Exists(string transactionHash);

        Task AddActivity(ActivityServiceModel model);

        Task SetActivityStatus(string transactionHash, ActivityStatus status);
    }
}
