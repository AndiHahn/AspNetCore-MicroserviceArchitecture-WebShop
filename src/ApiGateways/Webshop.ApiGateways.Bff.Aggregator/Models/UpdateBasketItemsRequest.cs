using System.Collections.Generic;

namespace Webshop.ApiGateways.Bff.Aggregator.Models
{
    public class UpdateBasketItemsRequest
    {
        public IEnumerable<BasketUpdateItem> UpdateItems { get; set; }
    }
}
