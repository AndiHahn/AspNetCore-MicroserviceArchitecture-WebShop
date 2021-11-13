using System;

namespace Webshop.WebApps.MvcClient.ViewModels
{
    public class CatalogItem
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public int AvailableStock { get; init; }
        public string PictureUri { get; set; }
    }
}
