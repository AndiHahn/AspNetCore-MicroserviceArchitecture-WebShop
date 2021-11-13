using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.Services.Basket.Api.Exceptions;
using Webshop.Services.Basket.Api.Infrastructure.Repositories;
using Webshop.Services.Basket.Api.Models;
using Webshop.Services.Basket.Api.Services;

namespace Webshop.Services.Basket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService basketService;
        private readonly IBasketRepository basketRepository;

        public BasketController(
            IBasketService basketService,
            IBasketRepository basketRepository)
        {
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
            this.basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        }

        [HttpGet]
        [ProducesResponseType(typeof(BasketData), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBasket()
        {
            var customers = basketRepository.GetAllCustomers();
            if (!customers.Any())
            {
                return Ok(new List<BasketItem>());
            }

            var basket = await basketRepository.GetBasketAsync(customers.First());
            return Ok(basket);
        }

        [HttpPost("checkout")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Checkout([FromBody] CheckoutBasketRequest checkoutRequest)
        {
            var customers = basketRepository.GetAllCustomers();
            if (!customers.Any())
            {
                throw new BadRequestException("No basket available.");
            }

            await basketService.CheckoutBasketAsync(customers.First(), checkoutRequest);

            return Accepted();
        }
    }
}
