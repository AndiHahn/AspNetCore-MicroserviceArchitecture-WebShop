using System.Collections.Generic;
using System.Threading.Tasks;
using Webshop.WebApps.MvcClient.ViewModels;

namespace Webshop.WebApps.MvcClient.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync();
    }
}
