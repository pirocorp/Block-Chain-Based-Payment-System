namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    public interface IBlockChainSignalRService
    {
        Task Run();
    }
}
