using System.Collections.Generic;

namespace Webshop.Services.Order.Api.Application.Models
{
    public class GetOrderDraftRequest
    {
        public string CustomerId { get; set; }
        public IEnumerable<BasketItem> Items { get; set; }
    }
}
