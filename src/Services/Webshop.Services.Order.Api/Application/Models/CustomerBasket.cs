using System.Collections.Generic;

namespace Webshop.Services.Order.Api.Application.Models
{
    public class CustomerBasket
    {
        public string CustomerId { get; set; }
        public IList<BasketItem> Items { get; set; }
    }
}
