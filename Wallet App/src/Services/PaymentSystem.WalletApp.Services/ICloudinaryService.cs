namespace PaymentSystem.WalletApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface ICloudinaryService
    {
        Task<IEnumerable<string>> Upload(ICollection<IFormFile> files);
    }
}
