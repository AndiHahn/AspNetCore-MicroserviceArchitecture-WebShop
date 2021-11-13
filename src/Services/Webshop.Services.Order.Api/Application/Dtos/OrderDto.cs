using System;
using System.Collections.Generic;
using Webshop.Services.Order.Api.Domain.OrderAggregate;

namespace Webshop.Services.Order.Api.Application.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; init; }
        public DateTime OrderDate { get; init; }
        public string Status { get; init; }
        public string Street { get; init; }
        public string City { get; init; }
        public string ZipCode { get; init; }
        public string Country { get; init; }
        public double TotalPrice { get; init; }
        public IEnumerable<OrderItem> OrderItems { get; init; }
    }
}
