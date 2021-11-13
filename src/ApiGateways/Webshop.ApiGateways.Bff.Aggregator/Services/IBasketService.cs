using System.Threading.Tasks;

namespace Webshop.ApiGateways.Bff.Aggregator.Services
{
    public interface IBasketService
    {
        Task<Models.BasketData> GetBasketAsync();
        Task UpdateBasketAsync(Models.BasketData basketData);
    }
}
