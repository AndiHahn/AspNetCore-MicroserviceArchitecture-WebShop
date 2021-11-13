using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Webshop.WebApps.MvcClient.Infrastructure;
using Webshop.WebApps.MvcClient.ViewModels;

namespace Webshop.WebApps.MvcClient.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient httpClient;
        private readonly string apiBaseUrl;

        public BasketService(
            HttpClient httpClient,
            IOptions<AppSettings> settings)
        {
            this.httpClient = httpClient;
            apiBaseUrl = settings.Value.ApiBaseUrl;
        }

        public async Task<Basket> GetBasketAsync()
        {
            string uri = Api.Basket.GetBasket(apiBaseUrl);

            var responseString = await httpClient.GetStringAsync(uri);

            var basket = JsonConvert.DeserializeObject<Basket>(responseString);

            return basket;
        }

        public async Task<Basket> UpdateQuantitiesAsync(Dictionary<Guid, int> quantities)
        {
            string uri = Api.Basket.UpdateBasket(apiBaseUrl);

            var basket = new
            {
                UpdateItems = quantities.Select(q =>
                    new
                    {
                        ItemId = q.Key,
                        NewQuantity = q.Value
                    }).ToList()
            };

            var content = CreateStringContent(basket);

            var response = await httpClient.PutAsync(uri, content);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Basket>(responseString);
        }

        public async Task AddProductToBasketAsync(Guid productId)
        {
            string uri = Api.Basket.AddItemToBasket(apiBaseUrl);

            var newItem = new
            {
                ProductId = productId,
                Quantity = 1
            };

            var content = CreateStringContent(newItem);

            await httpClient.PostAsync(uri, content);
        }

        public async Task CheckoutBasketAsync(Order model)
        {
            string uri = Api.Basket.CheckoutBasket(apiBaseUrl);

            var checkoutRequest = new
            {
                Buyer = model.Buyer,
                ZipCode = model.ZipCode,
                City = model.City,
                Street = model.Street,
                Country = model.Country,
                CreditCardHolder = model.CardHolderName,
                CreditCardNumber = model.CardNumber,
                CreditCardSecurityNumber = model.CardSecurityNumber,
                CreditCardExpiration = model.CardExpiration
            };

            var content = CreateStringContent(checkoutRequest);

            await httpClient.PostAsync(uri, content);
        }

        private StringContent CreateStringContent<T>(T data)
        {
            return new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }
    }
}
