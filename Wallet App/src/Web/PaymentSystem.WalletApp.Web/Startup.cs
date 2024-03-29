﻿namespace PaymentSystem.WalletApp.Web
{
    using System.Reflection;

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
    using PaymentSystem.WalletApp.Services.Data.Implementations;
    using PaymentSystem.WalletApp.Services.Data.Models;
    using PaymentSystem.WalletApp.Services.Implementations;
    using PaymentSystem.WalletApp.Services.Messaging;
    using PaymentSystem.WalletApp.Web.Extensions;
    using PaymentSystem.WalletApp.Web.Infrastructure.Options;
    using PaymentSystem.WalletApp.Web.ViewModels;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<EncryptionOptions>(this.configuration.GetSection("Encryption"));
            services.Configure<FingerprintOptions>(this.configuration.GetSection("Fingerprint"));
            services.Configure<SecretOptions>(this.configuration.GetSection("Secret"));
            services.Configure<WalletProviderOptions>(this.configuration.GetSection("WalletProvider"));

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services
                .AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

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

            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddSingleton(this.configuration);
            services.AddAutoMapper(
                typeof(ErrorViewModel).GetTypeInfo().Assembly,
                typeof(IServiceModel).GetTypeInfo().Assembly);

            services.AddCloudinary(this.configuration);

            // Additional Services
            services.AddTransient<IBlockChainGrpcService, BlockChainGrpcService>();
            services.AddSingleton<IBlockChainSignalRService, BlockChainSignalRService>();
            services.AddTransient<ICloudinaryService, CloudinaryService>();
            services.AddTransient<ISaltService, SaltService>();
            services.AddTransient<ISecurelyEncryptDataService, SecurelyEncryptDataService>();

            // Data repositories
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddDomainServices(typeof(IService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplyMigrations<ApplicationDbContext>();
            app.SeedData();
            app.UseBlockNotifications();

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
