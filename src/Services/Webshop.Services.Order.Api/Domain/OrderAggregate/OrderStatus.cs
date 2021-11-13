using System;
using System.Collections.Generic;
using System.Linq;

namespace Webshop.Services.Order.Api.Domain.OrderAggregate
{
    public class OrderStatus : Enumeration
    {
        public static OrderStatus Submitted = new(1, nameof(Submitted).ToLowerInvariant());
        public static OrderStatus AwaitingValidation = new(2, nameof(AwaitingValidation).ToLowerInvariant());
        public static OrderStatus StockConfirmed = new(3, nameof(StockConfirmed).ToLowerInvariant());
        public static OrderStatus Paid = new(4, nameof(Paid).ToLowerInvariant());
        public static OrderStatus Shipped = new(5, nameof(Shipped).ToLowerInvariant());
        public static OrderStatus Cancelled = new(6, nameof(Cancelled).ToLowerInvariant());

        public OrderStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderStatus> List() =>
            new[]
            {
                Submitted,
                AwaitingValidation,
                StockConfirmed,
                Paid,
                Shipped,
                Cancelled
            };

        public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException($"Status {name} is not available.", nameof(name));
            }

            return state;
        }

        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Status with id {id} is not available.", nameof(id));
            }

            return state;
        }
    }
}
