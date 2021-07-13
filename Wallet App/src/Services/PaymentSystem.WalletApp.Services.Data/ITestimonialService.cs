namespace PaymentSystem.WalletApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITestimonialService
    {
        Task<IEnumerable<T>> GetTestimonials<T>(int n = 0);
    }
}
