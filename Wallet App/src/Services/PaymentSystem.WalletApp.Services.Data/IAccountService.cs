namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    using PaymentSystem.Common.GrpcService;

    public interface IAccountService
    {
        Task<bool> Exists(string address);

        Task<bool> UserOwnsAccount(string userId, string address);

        Task<AccountCreationResponse> Create(string userId);
    }
}
