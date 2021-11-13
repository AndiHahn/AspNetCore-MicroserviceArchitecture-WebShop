using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.WebApps.MvcClient.Services;
using Webshop.WebApps.MvcClient.ViewModels;

namespace Webshop.WebApps.MvcClient.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IBasketService basketService;

        public OrderController(
            IOrderService orderService,
            IBasketService basketService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public async Task<IActionResult> Index(Order item)
        {
            var vm = await orderService.GetAllOrdersAsync();
            return View(vm);
        }

        public async Task<IActionResult> Detail(Guid orderId)
        {
            var order = await orderService.GetOrderById(orderId);
            return View(order);
        }

        public async Task<IActionResult> Create()
        {
            var basket = await basketService.GetBasketAsync();
            var order = await orderService.GetOrderDraftAsync(basket.Items);
            order.CardExpirationShortFormat();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await basketService.CheckoutBasketAsync(model);

                    return RedirectToAction("Index", "Catalog");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", $"It was not possible to create a new order, please try later on ({ex.GetType().Name} - {ex.Message})");
            }

            return View("Create", model);
        }
    }
}
