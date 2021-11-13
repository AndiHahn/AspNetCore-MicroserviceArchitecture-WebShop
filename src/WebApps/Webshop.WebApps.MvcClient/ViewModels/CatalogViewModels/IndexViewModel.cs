using System.Collections.Generic;

namespace Webshop.WebApps.MvcClient.ViewModels.CatalogViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<CatalogItem> CatalogItems { get; set; }
    }
}
