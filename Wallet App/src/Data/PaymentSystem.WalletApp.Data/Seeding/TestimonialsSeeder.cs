namespace PaymentSystem.WalletApp.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Models;

    public class TestimonialsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Testimonials.AnyAsync())
            {
                return;
            }

            var testimonials = new List<Testimonial>()
            {
                new Testimonial()
                {
                    Comment = "Easy to use, reasonably priced simply dummy text of the printing and typesetting industry. Quidam lisque persius interesset his et, in quot quidam possim iriure.",
                    Name = "Jay Shah",
                    UseCase = "Founder at Icomatic Pvt Ltd",
                },
                new Testimonial()
                {
                    Comment = "I am happy Working with printing and typesetting industry. Quidam lisque persius interesset his et, in quot quidam persequeris essent possim iriure.",
                    Name = "Patrick Cary",
                    UseCase = "Freelancer from USA",
                },
                new Testimonial()
                {
                    Comment = "Fast easy to use transfers to a different currency. Much better value that the banks.",
                    Name = "De Mortel",
                    UseCase = "Online Retail",
                },
                new Testimonial()
                {
                    Comment = "I have used them twice now. Good rates, very efficient service and it denies high street banks an undeserved windfall. Excellent.",
                    Name = "Chris Tom",
                    UseCase = "User from UK",
                },
                new Testimonial()
                {
                    Comment = "It's a real good idea to manage your money by payyed. The rates are fair and you can carry out the transactions without worrying!",
                    Name = "Mauri Lindberg",
                    UseCase = "Freelancer from Australia",
                },
                new Testimonial()
                {
                    Comment = "Only trying it out since a few days. But up to now excellent. Seems to work flawlessly. I'm only using it for sending money to friends at the moment.",
                    Name = "Dennis Jacques",
                    UseCase = "User from USA",
                },
            };

            await dbContext.AddRangeAsync(testimonials);
            await dbContext.SaveChangesAsync();
        }
    }
}
