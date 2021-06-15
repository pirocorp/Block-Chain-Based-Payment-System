namespace PaymentSystem.BlockChain.Services.Data
{
    using System.Threading.Tasks;

    public interface IAccountService
    {
        Task<bool> Exists(string address);

        Task Create(string address);

        Task Deposit(string address, double amount);

        Task<bool> TryWithdraw(string address, double amount);
    }
}
