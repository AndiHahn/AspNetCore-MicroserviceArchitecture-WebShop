using System;
using System.Collections.Generic;

namespace Webshop.ApiGateways.Bff.Aggregator.Models
{
    public class BasketData
    {
        public Guid CustomerId { get; set; }
        public IList<BasketDataItem> Items { get; set; } = new List<BasketDataItem>();
    }
}
