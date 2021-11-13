using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Webshop.WebApps.MvcClient.Infrastructure;
using Webshop.WebApps.MvcClient.ViewModels;

namespace Webshop.WebApps.MvcClient.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient httpClient;
        private readonly string apiBaseUrl;

        public OrderService(
            HttpClient httpClient,
            IOptions<AppSettings> settings)
        {
            this.httpClient = httpClient;
            apiBaseUrl = settings.Value.ApiBaseUrl;
        }

        public async Task<IList<Order>> GetAllOrdersAsync()
        {
            string uri = Api.Order.GetAllOrders(apiBaseUrl);

            var responseString = await httpClient.GetStringAsync(uri);

            var orders = JsonConvert.DeserializeObject<List<Order>>(responseString);

            return orders;
        }

        public async Task<Order> GetOrderById(System.Guid id)
        {
            string uri = Api.Order.GetById(apiBaseUrl, id);

            var responseString = await httpClient.GetStringAsync(uri);

            var order = JsonConvert.DeserializeObject<Order>(responseString);

            return order;
        }

        public async Task<Order> GetOrderDraftAsync(IEnumerable<BasketItem> basketItems)
        {
            string uri = Api.Order.GetDraft(apiBaseUrl);

            var getOrderDraftRequest = new
            {
                Items = basketItems
            };

            var content = CreateStringContent(getOrderDraftRequest);

            var response = await httpClient.PostAsync(uri, content);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Order>(responseString);
        }

        private StringContent CreateStringContent<T>(T data)
        {
            return new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }
    }
}
