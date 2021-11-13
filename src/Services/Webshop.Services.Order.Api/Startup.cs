using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Webshop.CrossCutting.MessageBus;
using Webshop.CrossCutting.MessageBus.Models;
using Webshop.Services.Order.Api.Application.Events;
using Webshop.Services.Order.Api.Extensions;
using Webshop.Services.Order.Api.Infrastructure;

namespace Webshop.Services.Order.Api
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
            string connectionString = Configuration.GetConnectionString("ApplicationDb");
            services.AddDbContext<OrderContext>(
                options => options.UseNpgsql(connectionString));

            services.RegisterMessageBusServices(Configuration.GetSection("RabbitMQ"));

            services.AddScoped<IIntegrationEventHandler<BasketCheckoutIntegrationEvent>, BasketCheckoutIntegrationEventHandler>();

            services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddRabbitMQ(
                    $"amqp://{Configuration["RabbitMQ:HostName"]}",
                    name: "rabbitmq-healthcheck",
                    tags: new[] { "rabbitmq" })
                .AddNpgSql(
                    connectionString,
                    name: "postgres-healthcheck",
                    tags: new[] { "postgres" });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Webshop.Services.Order.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Webshop.Services.Order.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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

            app.UseMessageBusReceiver();
        }
    }
}
