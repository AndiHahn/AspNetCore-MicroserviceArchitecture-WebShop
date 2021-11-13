using System;

namespace Webshop.Services.Order.Api.Domain.OrderAggregate
{
    public class OrderItem : Entity
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public double UnitPrice { get; private set; }
        public int Units { get; private set; }
        public string PictureUri { get; private set; }

        protected OrderItem()
        {
        }

        public OrderItem(
            Guid productId,
            string productName,
            double unitPrice,
            int units,
            string pictureUri)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException($"Parameter {nameof(productId)} must not be empty.", nameof(productId));
            }

            if (units < 0)
            {
                throw new ArgumentException("Units must be > 0", nameof(units));
            }

            this.Id = Guid.NewGuid();
            this.ProductId = productId;
            this.ProductName = productName;
            this.UnitPrice = unitPrice;
            this.Units = units;
            this.PictureUri = pictureUri;
        }

        public void AddUnits(int units)
        {
            if (units < 0)
            {
                throw new ArgumentException("Units must be > 0", nameof(units));
            }

            this.Units += units;
        }
    }
}
