using System;

namespace Webshop.ApiGateways.Bff.Aggregator.Models
{
    public class BasketUpdateItem
    {
        public Guid ItemId { get; set; }
        public int NewQuantity { get; set; }
    }
}
