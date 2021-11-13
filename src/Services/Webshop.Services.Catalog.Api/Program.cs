using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Webshop.Services.Catalog.Api.Infrastructure;

namespace Webshop.Services.Catalog.Api
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
                    .Enrich.WithProperty("Service", "Catalog")
                    .WriteTo.Console()
                    .WriteTo.Seq(seqServerUrl)
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                try
                {
                    Log.Information("Start migrate database...");

                    var context = services.GetRequiredService<CatalogContext>();
                    await context.Database.MigrateAsync();

                    Log.Information("Finished database migration");
                    Log.Information("Start seed database...");

                    await CatalogContextSeed.SeedAsync(context);

                    Log.Information("Finished seed database.");
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "Error migrating the database");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 80, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });
                        options.Listen(IPAddress.Any, 81, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http2;
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
