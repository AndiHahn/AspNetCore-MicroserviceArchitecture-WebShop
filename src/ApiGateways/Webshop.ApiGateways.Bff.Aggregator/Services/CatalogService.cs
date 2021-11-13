using System;
using System.Threading.Tasks;
using Webshop.ApiGateways.Bff.Aggregator.Exceptions;
using Webshop.ApiGateways.Bff.Aggregator.Models;
using static Catalog;

namespace Webshop.ApiGateways.Bff.Aggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogClient catalogClient;

        public CatalogService(
            CatalogClient catalogClient)
        {
            this.catalogClient = catalogClient ?? throw new ArgumentNullException(nameof(catalogClient));
        }

        public async Task<CatalogItem> GetCatalogItemByIdAsync(Guid id)
        {
            var request = new CatalogItemByIdRequest
            {
                Id = id.ToString()
            };

            var response = await catalogClient.GetItemByIdAsync(request);
            if (response == null)
            {
                throw new NotFoundException($"Product with id {id} not found");
            }

            return new CatalogItem
            {
                Id = new Guid(response.Id),
                Name = response.Name,
                Description = response.Description,
                Price = response.Price,
                AvailableStock = response.AvailableStock,
                PictureUri = response.PictureUri
            };
        }
    }
}
