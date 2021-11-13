using System.Threading.Tasks;
using Webshop.Services.Basket.Api.Models;

namespace Webshop.Services.Basket.Api.Services
{
    public interface IBasketService
    {
        Task CheckoutBasketAsync(string customerId, CheckoutBasketRequest checkoutRequest);
    }
}
