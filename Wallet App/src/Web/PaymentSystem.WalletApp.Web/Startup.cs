namespace PaymentSystem.WalletApp.Web
{
    using System.Reflection;

    using CloudinaryDotNet;
    using Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using PaymentSystem.Common.Extensions;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Common;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Messaging;
    using PaymentSystem.WalletApp.Web.Extensions;
    using PaymentSystem.WalletApp.Web.ViewModels;
    using Services.Data.Models;

    public class Startup
    {
        private const string MyCorsPolicy = "MyCors";
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<EncryptionOptions>(this.configuration.GetSection("Encryption"));

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services
                .AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //services
            //    .AddIdentity<ApplicationUser, ApplicationRole>(IdentityOptionsProvider.GetIdentityOptions)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services
                .AddControllersWithViews(
                    options =>
                        {
                            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        })
                .AddRazorRuntimeCompilation();

            //services.AddCors(options =>
            //{
            //    options.AddPolicy(
            //        name: MyCorsPolicy,
            //        builder =>
            //        {
            //            builder
            //                .WithOrigins(
            //                    "https://192.168.1.5",
            //                    "https://loclahost")
            //                .AllowAnyHeader()
            //                .AllowAnyMethod()
            //                .AllowCredentials();
            //        });
            //});

            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddSingleton(this.configuration);
            services.AddAutoMapper(
                typeof(ErrorViewModel).GetTypeInfo().Assembly,
                typeof(IServiceModel).GetTypeInfo().Assembly);

            services.AddCloudinary(this.configuration);

            // External Services
            services.AddTransient<ICloudinaryService, CloudinaryService>();

            // Data repositories
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<ISaltService, SaltService>();
            services.AddTransient<ISecurelyEncryptDataService, SecurelyEncryptDataService>();
            services.AddTransient<IEmailSender, NullMessageSender>();
            services.AddTransient<ITestimonialService, TestimonialService>();
            services.AddTransient<ICreditCardService, CreditCardService>();
            services.AddTransient<IBankAccountService, BankAccountService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplyMigrations<ApplicationDbContext>();
            app.SeedData();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            //app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        
                        endpoints
                            .MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}")
                            /*.RequireCors(MyCorsPolicy)*/;
                        
                        endpoints.MapRazorPages();
                    });
        }
    }
}
