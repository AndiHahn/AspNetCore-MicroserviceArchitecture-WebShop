using System.Collections.Generic;
using System.Threading.Tasks;
using Webshop.Services.Basket.Api.Models;

namespace Webshop.Services.Basket.Api.Infrastructure.Repositories
{
    public interface IBasketRepository
    {
        Task<BasketData> GetBasketAsync(string customerId);
        Task<BasketData> UpdateBasketAsync(BasketData basket);
        Task DeleteBasketAsync(string customerId);
        IEnumerable<string> GetAllCustomers();
    }
}
