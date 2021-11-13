using System;

namespace Webshop.ApiGateways.Bff.Aggregator.Models
{
    public class CatalogItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int AvailableStock { get; set; }
        public string PictureUri { get; set; }
    }
}
