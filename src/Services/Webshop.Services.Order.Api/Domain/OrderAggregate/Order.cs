using System;
using System.Collections.Generic;
using System.Linq;

namespace Webshop.Services.Order.Api.Domain.OrderAggregate
{
    public class Order : Entity
    {
        public int StatusId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public Address Address { get; private set; }

        public OrderStatus Status { get; }
        public IReadOnlyCollection<OrderItem> OrderItems => orderItems.AsReadOnly();

        private readonly List<OrderItem> orderItems = new List<OrderItem>();
        private bool isDraft = false;

        protected Order()
        {
        }

        public Order(Address address)
        {
            this.Id = Guid.NewGuid();
            this.OrderDate = DateTime.UtcNow;
            this.Address = address;
            this.StatusId = OrderStatus.Submitted.Id;
        }

        public static Order NewDraft()
        {
            var order = new Order
            {
                isDraft = true
            };

            return order;
        }

        public void AddOrderItem(Guid productId, string productName, double unitPrice, int units, string pictureUri)
        {
            var existingItem = orderItems.SingleOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.AddUnits(units);
            }
            else
            {
                var newItem = new OrderItem(productId, productName, unitPrice, units, pictureUri);
                orderItems.Add(newItem);
            }
        }

        public double GetTotalPrice()
        {
            return orderItems.Sum(i => i.UnitPrice * i.Units);
        }
    }
}
