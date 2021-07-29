namespace PaymentSystem.WalletApp.Services
{
    using System.Threading.Tasks;

    public interface IBlockChainSignalRService
    {
        Task Run();
    }
}
