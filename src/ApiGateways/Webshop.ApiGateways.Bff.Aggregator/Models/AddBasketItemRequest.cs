using System;

namespace Webshop.ApiGateways.Bff.Aggregator.Models
{
    public class AddBasketItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
