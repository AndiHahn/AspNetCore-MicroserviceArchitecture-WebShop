using System;
using System.Threading.Tasks;
using static Basket;

namespace Webshop.ApiGateways.Bff.Aggregator.Services
{
    public class BasketService : IBasketService
    {
        private readonly BasketClient basketClient;

        public BasketService(BasketClient basketClient)
        {
            this.basketClient = basketClient ?? throw new ArgumentNullException(nameof(basketClient));
        }

        public async Task<Models.BasketData> GetBasketAsync()
        {
            var request = new GetBasketRequest();

            var response = await basketClient.GetBasketAsync(request);

            var basket = new Models.BasketData
            {
                CustomerId = new Guid(response.CustomerId)
            };

            foreach (var item in response.Items)
            {
                basket.Items.Add(new Models.BasketDataItem
                {
                    Id = new Guid(item.Id),
                    ProductId = new Guid(item.ProductId),
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    PictureUri = item.PictureUri
                });
            }

            return basket;
        }

        public async Task UpdateBasketAsync(Models.BasketData basketData)
        {
            var request = new UpdateBasketRequest
            {
                CustomerId = basketData.CustomerId.ToString()
            };

            foreach (var item in basketData.Items)
            {
                request.Items.Add(new BasketItem
                {
                    Id = item.Id.ToString(),
                    ProductId = item.ProductId.ToString(),
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    PictureUri = item.PictureUri
                });
            }

            await basketClient.UpdateBasketAsync(request);
        }
    }
}
