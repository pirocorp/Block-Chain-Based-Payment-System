namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data;

    public class TestimonialService : ITestimonialService
    {
        private readonly ApplicationDbContext dbContext;

        public TestimonialService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> GetTestimonials<T>(int n = 0)
            => await this.dbContext.Testimonials
                .OrderBy(t => Guid.NewGuid())
                .To<T>()
                .Take(n)
                .ToListAsync();
    }
}