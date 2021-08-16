namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using System;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.WalletApp.Data;

    public static class ApplicationDbContextMocks
    {
        public static ApplicationDbContext Instance
        {
            get
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

                return new ApplicationDbContext(options);
            }
        }
    }
}
