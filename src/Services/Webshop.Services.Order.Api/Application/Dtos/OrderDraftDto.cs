using System.Collections.Generic;
using System.Linq;

namespace Webshop.Services.Order.Api.Application.Dtos
{
    public class OrderDraftDto
    {
        public IEnumerable<OrderItemDto> OrderItems { get; init; }
        public double TotalPrice { get; init; }

        public static OrderDraftDto FromOrder(Domain.OrderAggregate.Order order)
        {
            return new OrderDraftDto
            {
                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Units = i.Units,
                    PictureUri = i.PictureUri
                }),
                TotalPrice = order.GetTotalPrice()
            };
        }
    }
}
