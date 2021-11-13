using System.Threading.Tasks;
using Webshop.CrossCutting.MessageBus.Models;
using Webshop.Services.Order.Api.Domain.OrderAggregate;
using Webshop.Services.Order.Api.Infrastructure;

namespace Webshop.Services.Order.Api.Application.Events
{
    public class BasketCheckoutIntegrationEventHandler : IIntegrationEventHandler<BasketCheckoutIntegrationEvent>
    {
        private readonly OrderContext context;

        public BasketCheckoutIntegrationEventHandler(
            OrderContext context)
        {
            this.context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        public async Task Handle(BasketCheckoutIntegrationEvent @event)
        {
            var address = new Address(@event.Street, @event.City, @event.Country, @event.ZipCode);
            var order = new Domain.OrderAggregate.Order(address);

            foreach (var item in @event.Basket.Items)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity, item.PictureUri);
            }

            await context.Orders.AddAsync(order);

            await context.SaveChangesAsync();
        }
    }
}
