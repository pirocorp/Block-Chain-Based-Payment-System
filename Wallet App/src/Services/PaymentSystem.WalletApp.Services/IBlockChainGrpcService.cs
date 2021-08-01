namespace PaymentSystem.WalletApp.Services
{
    using System.Threading.Tasks;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.GrpcService;

    public interface IBlockChainGrpcService
    {
        Task<AccountCreationResponse> CreateAccount();

        Task<BoolResponse> DeleteAccount(string address);

        Task<TransactionResponse> AddTransactionToPool(SendRequest sendRequest);

        Task<Block> GetBlock(long height);
    }
}
