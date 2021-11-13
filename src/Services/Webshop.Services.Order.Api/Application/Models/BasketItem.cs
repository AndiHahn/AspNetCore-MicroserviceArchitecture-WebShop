using System;

namespace Webshop.Services.Order.Api.Application.Models
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUri { get; set; }
    }
}
