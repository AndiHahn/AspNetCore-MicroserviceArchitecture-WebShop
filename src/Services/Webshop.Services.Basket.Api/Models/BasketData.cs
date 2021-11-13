using System.Collections.Generic;

namespace Webshop.Services.Basket.Api.Models
{
    public class BasketData
    {
        public string CustomerId { get; set; }
        public ICollection<BasketDataItem> Items { get; set; } = new List<BasketDataItem>();
    }
}
