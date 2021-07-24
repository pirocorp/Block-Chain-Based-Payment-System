namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    using PaymentSystem.Common.Utilities;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.Infrastructure.Options;

    public class FingerprintService : IFingerprintService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IOptions<FingerprintOptions> fingerprintOptions;

        public FingerprintService(
            ApplicationDbContext dbContext,
            IOptions<FingerprintOptions> fingerprintOptions)
        {
            this.dbContext = dbContext;
            this.fingerprintOptions = fingerprintOptions;
        }

        public async Task<bool> Exists(string pan)
        {
            var hash = this.GetPanHash(pan);

            return await this.dbContext.Fingerprints.AnyAsync(f => f.Stamp == hash);
        }

        public async Task CreateFingerprint(string pan)
        {
            var hash = this.GetPanHash(pan);
            var fingerprint = new Fingerprint() { Stamp = hash };

            await this.dbContext.AddAsync(fingerprint);
            await this.dbContext.SaveChangesAsync();
        }

        private string GetPanHash(string pan)
        {
            var data = pan + this.fingerprintOptions.Value.Key;
            var hash = BlockChainHashing.GenerateHash(data);

            return hash;
        }
    }
}
