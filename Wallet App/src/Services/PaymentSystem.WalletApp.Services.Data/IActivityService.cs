namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    using PaymentSystem.WalletApp.Services.Data.Models.Activities;

    public interface IActivityService
    {
        Task AddActivity(ActivityServiceModel model);
    }
}
