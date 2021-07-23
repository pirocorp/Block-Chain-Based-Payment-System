namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Threading.Tasks;

    public interface IFingerprintService
    {
        Task<bool> Exists(string pan);

        Task CreateFingerprint(string pan);
    }
}
