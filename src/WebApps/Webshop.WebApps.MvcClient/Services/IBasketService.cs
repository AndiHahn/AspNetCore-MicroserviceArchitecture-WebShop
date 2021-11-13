using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webshop.WebApps.MvcClient.ViewModels;

namespace Webshop.WebApps.MvcClient.Services
{
    public interface IBasketService
    {
        Task<Basket> GetBasketAsync();
        Task<Basket> UpdateQuantitiesAsync(Dictionary<Guid, int> quantities);
        Task AddProductToBasketAsync(Guid productId);
        Task CheckoutBasketAsync(Order model);
    }
}
