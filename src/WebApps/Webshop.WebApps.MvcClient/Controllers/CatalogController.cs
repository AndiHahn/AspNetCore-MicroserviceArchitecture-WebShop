using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.WebApps.MvcClient.Services;
using Webshop.WebApps.MvcClient.ViewModels.CatalogViewModels;

namespace Webshop.WebApps.MvcClient.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            this.catalogService = catalogService ?? throw new System.ArgumentNullException(nameof(catalogService));
        }

        public async Task<IActionResult> Index([FromQuery] string errorMsg)
        {
            var catalogItems = await catalogService.GetCatalogItemsAsync();

            var viewModel = new IndexViewModel
            {
                CatalogItems = catalogItems
            };

            ViewBag.BasketInoperativeMsg = errorMsg;

            return View(viewModel);
        }
    }
}
