using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using Webshop.Services.Basket.Api.Models;

namespace Webshop.Services.Basket.Api.Infrastructure.Repositories
{
    public class RedisBasketRepository : IBasketRepository
    {
        private readonly ConnectionMultiplexer connection;
        private readonly IDatabase database;

        public RedisBasketRepository(ConnectionMultiplexer connection)
        {
            this.connection = connection ?? throw new System.ArgumentNullException(nameof(connection));
            database = connection.GetDatabase();
        }

        public async Task<BasketData> GetBasketAsync(string customerId)
        {
            var data = await database.StringGetAsync(customerId);
            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<BasketData>(data);
        }

        public async Task<BasketData> UpdateBasketAsync(BasketData basket)
        {
            var itemCreated = await database.StringSetAsync(basket.CustomerId, JsonConvert.SerializeObject(basket));
            if (!itemCreated)
            {
                return null;
            }

            return await GetBasketAsync(basket.CustomerId);
        }

        public async Task DeleteBasketAsync(string customerId)
        {
            await database.KeyDeleteAsync(customerId);
        }

        public IEnumerable<string> GetAllCustomers()
        {
            var server = GetServer();
            var data = server.Keys();

            return data?.Select(k => k.ToString());
        }

        private IServer GetServer()
        {
            var endpoint = connection.GetEndPoints();
            return connection.GetServer(endpoint.First());
        }
    }
}
