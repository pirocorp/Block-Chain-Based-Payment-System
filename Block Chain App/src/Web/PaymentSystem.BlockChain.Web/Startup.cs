namespace PaymentSystem.BlockChain.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using PaymentSystem.BlockChain.Data;
    using PaymentSystem.BlockChain.Services;
    using PaymentSystem.BlockChain.Services.Data;
    using PaymentSystem.BlockChain.Services.Hubs;
    using PaymentSystem.BlockChain.Web.Extensions;
    using PaymentSystem.BlockChain.Web.Infrastructure;
    using PaymentSystem.Common;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddGrpc();
            services.AddGrpcReflection(); // Used to test gRPC with grpcurl CLI.

            services.AddSignalR();

            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));

            services.AddAutoMapper();

            // Domain Services
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<IBlockChainService, BlockChainService>();
            services.AddTransient<BlockChainCommunicationService>();
            services.AddSingleton<ITransactionPool, TransactionPool>();
            services.AddSingleton<ICancelTransactionPool, CancelTransactionPool>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplyMigrations();
            app.SeedData();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapGrpcService<BlockChainCommunicationService>();
                    endpoints.MapHub<BroadcastHub>(GlobalConstants.PushNotificationUrl);

                    if (env.IsDevelopment())
                    {
                        // Used to test gRPC with grpcurl CLI.
                        endpoints.MapGrpcReflectionService();

                        endpoints.MapGet("/", async context =>
                        {
                            await context.Response.WriteAsync(
                                "Communication with gRPC endpoints" +
                                " must be made through a gRPC client.");
                        });
                    }
                });
        }
    }
}
