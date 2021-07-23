namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Threading.Tasks;

    using Models;

    public interface IAccountService
    {
        Task<bool> Exists(string address);

        Task<AccountServiceModel> Create();

        Task<bool> TryDeposit(string address, double amount);

        Task<bool> TryWithdraw(string address, double amount);
    }
}
