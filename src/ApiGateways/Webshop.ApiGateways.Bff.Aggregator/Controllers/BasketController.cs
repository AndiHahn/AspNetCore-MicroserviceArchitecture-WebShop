using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.ApiGateways.Bff.Aggregator.Exceptions;
using Webshop.ApiGateways.Bff.Aggregator.Models;
using Webshop.ApiGateways.Bff.Aggregator.Services;

namespace Webshop.ApiGateways.Bff.Aggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;

        public BasketController(
            ICatalogService catalogService,
            IBasketService basketService)
        {
            this.catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        [HttpPost("items")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> AddItemToBasket([FromBody] AddBasketItemRequest request)
        {
            var product = await catalogService.GetCatalogItemByIdAsync(request.ProductId);

            var basket = await basketService.GetBasketAsync();
            if (basket == null)
            {
                throw new NotFoundException("Basket not found.");
            }

            var basketItem = basket.Items.SingleOrDefault(i => i.ProductId == request.ProductId);
            if (basketItem != null)
            {
                basketItem.Quantity += request.Quantity;
            }
            else
            {
                basket.Items.Add(new BasketDataItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = request.Quantity,
                    PictureUri = product.PictureUri
                });
            }

            await basketService.UpdateBasketAsync(basket);

            return NoContent();
        }

        [HttpPut("items")]
        [ProducesResponseType(typeof(BasketData), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateQuantitiesAsync([FromBody] UpdateBasketItemsRequest updateData)
        {
            var basket = await basketService.GetBasketAsync();
            if (basket == null)
            {
                throw new NotFoundException("Basket not found.");
            }

            foreach (var updateItem in updateData.UpdateItems)
            {
                var basketItem = basket.Items.SingleOrDefault(i => i.Id == updateItem.ItemId);
                if (basketItem == null)
                {
                    throw new BadRequestException($"Basket item with id {updateItem.ItemId} not available.");
                }

                if (updateItem.NewQuantity > 0)
                {
                    basketItem.Quantity = updateItem.NewQuantity;
                }
                else
                {
                    basket.Items.Remove(basketItem);
                }
            }

            await basketService.UpdateBasketAsync(basket);

            return Ok(basket);
        }
    }
}
