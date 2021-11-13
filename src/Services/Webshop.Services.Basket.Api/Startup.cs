using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using Webshop.CrossCutting.MessageBus;
using Webshop.Services.Basket.Api.Exceptions;
using Webshop.Services.Basket.Api.Grpc;
using Webshop.Services.Basket.Api.Infrastructure.Repositories;
using Webshop.Services.Basket.Api.Services;

namespace Webshop.Services.Basket.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                string connectionString = configuration.GetSection("Redis").GetSection("ConnectionString").Value;

                var redisConfig = ConfigurationOptions.Parse(connectionString, true);
                redisConfig.ResolveDns = true;

                return ConnectionMultiplexer.Connect(redisConfig);
            });

            services.AddTransient<IBasketRepository, RedisBasketRepository>();
            services.AddScoped<IBasketService, BasketService>();

            services.RegisterMessageBusServices(Configuration.GetSection("RabbitMQ"));

            services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddRedis(
                    Configuration["Redis:ConnectionString"],
                    name: "redis-healthcheck",
                    tags: new[] { "redis" })
                .AddRabbitMQ(
                    $"amqp://{Configuration["RabbitMQ:HostName"]}",
                    name: "rabbitmq-healthcheck",
                    tags: new[] { "rabbitmq" });

            services.AddProblemDetails(opts =>
            {
                opts.Map<BadRequestException>(ex =>
                    new Microsoft.AspNetCore.Mvc.ProblemDetails
                    {
                        Title = "Bad Request",
                        Detail = ex.Message,
                        Status = 400,
                        Type = "https://httpstatuses.com/400"
                    });

                opts.Map<NotFoundException>(ex =>
                    new Microsoft.AspNetCore.Mvc.ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = ex.Message,
                        Status = 404,
                        Type = "https://httpstatuses.com/404"
                    });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Webshop.Services.Basket", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Webshop.Services.Basket v1"));
            }

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<BasketGrpcService>();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = p => p.Name.Contains("self")
                });
            });
        }
    }
}
