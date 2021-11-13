using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Webshop.CrossCutting.MessageBus;
using Webshop.CrossCutting.MessageBus.Models;
using Webshop.Services.Order.Api.Application.Events;

namespace Webshop.Services.Order.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseMessageBusReceiver(this IApplicationBuilder app)
        {
            var messageBus = app.ApplicationServices.GetRequiredService<IMessageReceiver>();

            messageBus.Subscribe<BasketCheckoutIntegrationEvent, IIntegrationEventHandler<BasketCheckoutIntegrationEvent>>();
        }
    }
}
