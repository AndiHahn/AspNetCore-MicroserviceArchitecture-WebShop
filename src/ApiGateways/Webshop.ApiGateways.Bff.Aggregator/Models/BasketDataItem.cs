using System;

namespace Webshop.ApiGateways.Bff.Aggregator.Models
{
    public class BasketDataItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUri { get; set; }
    }
}
