using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.WebApps.MvcClient.Services;
using Webshop.WebApps.MvcClient.ViewModels;

namespace Webshop.WebApps.MvcClient.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketService basketService;

        public BasketController(IBasketService basketService)
        {
            this.basketService = basketService ?? throw new System.ArgumentNullException(nameof(basketService));
        }

        public async Task<IActionResult> Index([FromQuery] string errorMsg)
        {
            var basket = new ViewModels.Basket();

            try
            {
                basket = await basketService.GetBasketAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            ViewBag.BasketInoperativeMsg = errorMsg;

            return View(basket);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<Guid, int> quantities, string action)
        {
            try
            {
                var basket = await basketService.UpdateQuantitiesAsync(quantities);
                if (!basket.Items.Any())
                {
                    return RedirectToAction("Index", "Catalog");
                }

                if (action == "Checkout")
                {
                    return RedirectToAction("Create", "Order");
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return View();
        }

        public async Task<IActionResult> AddToBasket(CatalogItem catalogItem)
        {
            try
            {
                if (catalogItem?.Id != Guid.Empty)
                {
                    await basketService.AddProductToBasketAsync(catalogItem.Id);
                }

                return RedirectToAction("Index", "Catalog");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return RedirectToAction("Index", "Catalog", new { errorMsg = ViewBag.InoperativeMsg });
        }

        private void HandleException(Exception ex)
        {
            ViewBag.BasketInoperativeMsg = $"Basket Service is inoperative {ex.GetType().Name} - {ex.Message}";
        }
    }
}
