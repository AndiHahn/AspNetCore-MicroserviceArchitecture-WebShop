using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.WebApps.MvcClient.Services;
using Webshop.WebApps.MvcClient.ViewModels.BasketViewModels;

namespace Webshop.WebApps.MvcClient.ViewComponents
{
    public class Basket : ViewComponent
    {
        private readonly IBasketService basketService;

        public Basket(IBasketService basketService)
        {
            this.basketService = basketService ?? throw new System.ArgumentNullException(nameof(basketService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new BasketComponentViewModel();

            try
            {
                int nrOfItems = (await basketService.GetBasketAsync()).Items.Count;
                vm.ItemsCount = nrOfItems;
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
