using System;

namespace Webshop.WebApps.MvcClient.Infrastructure
{
    public static class Api
    {
        public static class Catalog
        {
            public static string GetAllCatalogItems(string baseUri)
            {
                return $"{baseUri}/catalog";
            }
        }

        public static class Basket
        {
            public static string GetBasket(string baseUri)
            {
                return $"{baseUri}/basket";
            }

            public static string UpdateBasket(string baseUri)
            {
                return $"{baseUri}/basket/items";
            }

            public static string AddItemToBasket(string baseUri)
            {
                return $"{baseUri}/basket/items";
            }

            public static string CheckoutBasket(string baseUri)
            {
                return $"{baseUri}/basket/checkout";
            }
        }

        public static class Order
        {
            public static string GetAllOrders(string baseUri)
            {
                return $"{baseUri}/order";
            }

            public static string GetById(string baseUri, Guid id)
            {
                return $"{baseUri}/order/{id}";
            }

            public static string GetDraft(string baseUri)
            {
                return $"{baseUri}/order/draft";
            }
        }
    }
}
