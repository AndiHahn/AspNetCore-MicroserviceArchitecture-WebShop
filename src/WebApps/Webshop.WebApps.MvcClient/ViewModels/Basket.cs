using System;
using System.Collections.Generic;
using System.Linq;

namespace Webshop.WebApps.MvcClient.ViewModels
{
    public class Basket
    {
        public IList<BasketItem> Items { get; set; }
        public string CustomerId { get; set; }

        public decimal Total()
        {
            return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity), 2);
        }
    }
}
