using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.WebApps.MvcClient.Services;

namespace Webshop.WebApps.MvcClient.ViewComponents
{
    public class BasketList : ViewComponent
    {
        private readonly IBasketService basketService;

        public BasketList(IBasketService basketService)
        {
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new ViewModels.Basket();

            try
            {
                vm = await basketService.GetBasketAsync();
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.BasketInoperativeMsg = $"Basket Service is inoperative, please try later on. ({ex.GetType().Name} - {ex.Message}))";
            }

            return View(vm);
        }
    }
}
