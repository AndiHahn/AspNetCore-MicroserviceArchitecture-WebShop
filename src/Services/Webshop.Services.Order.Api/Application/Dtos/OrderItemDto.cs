using System;

namespace Webshop.Services.Order.Api.Application.Dtos
{
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Units { get; set; }
        public string PictureUri { get; set; }
    }
}
