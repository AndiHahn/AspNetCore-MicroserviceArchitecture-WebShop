using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webshop.Services.Order.Api.Domain.OrderAggregate;

namespace Webshop.Services.Order.Api.Infrastructure
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext context)
        {
            if (!await context.OrderStatus.AnyAsync())
            {
                await context.OrderStatus.AddRangeAsync(OrderStatus.List());
                await context.SaveChangesAsync();
            }
        }
    }
}
