using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webshop.ApiGateways.Bff.Aggregator.Exceptions;
using Webshop.ApiGateways.Bff.Aggregator.Infrastructure;
using Webshop.ApiGateways.Bff.Aggregator.Services;
using static Basket;
using static Catalog;

namespace Webshop.ApiGateways.Bff.Aggregator.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static void AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddGrpcServices(configuration);
            services.AddProblemDetails(configuration);
        }

        private static void AddGrpcServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<GrpcExceptionInterceptor>();

            services.AddScoped<ICatalogService, CatalogService>();
            services.AddGrpcClient<CatalogClient>(options =>
            {
                string url = configuration.GetSection("ServiceUrls").GetSection("GrpcCatalog").Value;
                options.Address = new System.Uri(url);
            }).AddInterceptor<GrpcExceptionInterceptor>();

            services.AddScoped<IBasketService, BasketService>();
            services.AddGrpcClient<BasketClient>(options =>
            {
                string url = configuration.GetSection("ServiceUrls").GetSection("GrpcBasket").Value;
                options.Address = new System.Uri(url);
            }).AddInterceptor<GrpcExceptionInterceptor>();
        }

        private static IServiceCollection AddProblemDetails(
            this IServiceCollection services,
            IConfiguration configuration)
        {
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

            return services;
        }
    }
}
