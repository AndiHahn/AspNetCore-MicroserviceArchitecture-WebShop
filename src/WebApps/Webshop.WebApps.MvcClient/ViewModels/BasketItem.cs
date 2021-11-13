using System;

namespace Webshop.WebApps.MvcClient.ViewModels
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUri { get; set; }
    }
}
