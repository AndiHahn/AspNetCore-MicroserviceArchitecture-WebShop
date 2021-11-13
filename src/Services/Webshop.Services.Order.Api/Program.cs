using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Serilog;
using Webshop.Services.Order.Api.Infrastructure;

namespace Webshop.Services.Order.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var configuration = services.GetRequiredService<IConfiguration>();
                string seqServerUrl = configuration["Serilog:SeqServerUrl"];

                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Service", "Order")
                    .WriteTo.Console()
                    .WriteTo.Seq(seqServerUrl)
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                var retry = Policy.Handle<Exception>()
                    .WaitAndRetry(new[] {
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(20)
                    });

                await retry.Execute(async () =>
                {
                    Log.Information("Start migrate database...");

                    var context = services.GetRequiredService<OrderContext>();
                    await context.Database.MigrateAsync();

                    Log.Information("Finished database migration");
                    Log.Information("Start seed database...");

                    await OrderContextSeed.SeedAsync(context);

                    Log.Information("Finished seed database.");
                });
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
