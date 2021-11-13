using System;
using System.Threading.Tasks;
using Webshop.ApiGateways.Bff.Aggregator.Models;

namespace Webshop.ApiGateways.Bff.Aggregator.Services
{
    public interface ICatalogService
    {
        Task<CatalogItem> GetCatalogItemByIdAsync(Guid id);
    }
}
