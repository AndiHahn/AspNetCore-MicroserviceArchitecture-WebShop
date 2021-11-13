using System;

namespace Webshop.WebApps.MvcClient.ViewModels
{
    public class OrderItem
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; }
        public double UnitPrice { get; init; }
        public int Units { get; init; }
        public string PictureUri { get; init; }
    }
}
