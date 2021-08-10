namespace PaymentSystem.WalletApp.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using WalletApp.Common;

    public class AdminSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedAdmin(userManager, dbContext);
        }

        private async Task SeedAdmin(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            var adminUsername = "Admin";

            var admin = await GetAdmin(dbContext, adminUsername);

            if (admin != null)
            {
                return;
            }

            var user = new ApplicationUser
            {
                UserName = adminUsername,
                Email = "admin@pirocoin.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            await userManager.CreateAsync(user, "123456");

            admin = await GetAdmin(dbContext, adminUsername);
            await userManager.AddToRoleAsync(admin, WalletConstants.AdministratorRoleName);
        }

        private static async Task<ApplicationUser> GetAdmin(ApplicationDbContext dbContext, string adminUsername)
        {
            var admin = await dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == adminUsername);
            return admin;
        }
    }
}
