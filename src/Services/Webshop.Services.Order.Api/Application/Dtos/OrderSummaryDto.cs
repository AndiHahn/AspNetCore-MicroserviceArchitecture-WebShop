using System;

namespace Webshop.Services.Order.Api.Application.Dtos
{
    public class OrderSummaryDto
    {
        public Guid Id { get; init; }
        public DateTime OrderDate { get; init; }
        public string Status { get; init; }
        public double TotalPrice { get; init; }
    }
}
