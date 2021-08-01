namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    public interface ISettingService
    {
        Task<string> GetSetting(string key);

        Task<string> GetBlockChainAddress();

        Task<string> GetBlockPublicKey();

    }
}
