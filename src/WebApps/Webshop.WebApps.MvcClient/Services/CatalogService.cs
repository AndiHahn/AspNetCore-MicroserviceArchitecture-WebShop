using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Webshop.WebApps.MvcClient.Infrastructure;
using Webshop.WebApps.MvcClient.ViewModels;

namespace Webshop.WebApps.MvcClient.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient httpClient;
        private readonly string apiBaseUrl;

        public CatalogService(
            HttpClient httpClient,
            IOptions<AppSettings> settings)
        {
            this.httpClient = httpClient;
            apiBaseUrl = settings.Value.ApiBaseUrl;
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync()
        {
            string uri = Api.Catalog.GetAllCatalogItems(apiBaseUrl);

            var responseString = await httpClient.GetStringAsync(uri);

            var catalogItems = JsonConvert.DeserializeObject<IEnumerable<CatalogItem>>(responseString);

            return catalogItems;
        }
    }
}
