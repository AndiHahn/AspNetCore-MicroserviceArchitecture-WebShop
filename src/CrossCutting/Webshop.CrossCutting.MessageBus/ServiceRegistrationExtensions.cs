using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webshop.CrossCutting.MessageBus.RabbitMQ;

namespace Webshop.CrossCutting.MessageBus
{
    public static class ServiceRegistrationExtensions
    {
        public static void RegisterMessageBusServices(
            this IServiceCollection services,
            IConfiguration busConfiguration)
        {
            services.AddSingleton<IMessageSender, RabbitMqSender>();
            services.AddSingleton<IMessageReceiver, RabbitMqReceiver>();
            services.AddSingleton<ISubscriptionsManager, InMemorySubscriptionsManager>();
            services.AddSingleton<IRabbitMqConnection, DefaultRabbitMqConnection>();

            services.Configure<RabbitMqConfiguration>(busConfiguration);
        }
    }
}
