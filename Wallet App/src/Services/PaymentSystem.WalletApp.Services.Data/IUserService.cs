namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<T> GetUser<T>(string id);
    }
}
