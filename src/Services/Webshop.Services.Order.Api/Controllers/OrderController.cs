using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Services.Order.Api.Application.Dtos;
using Webshop.Services.Order.Api.Application.Models;
using Webshop.Services.Order.Api.Infrastructure;

namespace Webshop.Services.Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderContext dbContext;

        public OrderController(OrderContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status.Name,
                Street = order.Address.Street,
                City = order.Address.City,
                ZipCode = order.Address.ZipCode,
                Country = order.Address.Country,
                TotalPrice = order.GetTotalPrice(),
                OrderItems = order.OrderItems
            });
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Status)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status.Name,
                TotalPrice = o.OrderItems.Select(i => i.UnitPrice * i.Units).Sum()
            });

            return Ok(orderDtos);
        }

        [HttpPost("draft")]
        [ProducesResponseType(typeof(OrderDraftDto), StatusCodes.Status200OK)]
        public IActionResult GetDraftFromBasketItems([FromBody] GetOrderDraftRequest orderDraftRequest)
        {
            var order = Domain.OrderAggregate.Order.NewDraft();

            foreach (var item in orderDraftRequest.Items)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity, item.PictureUri);
            }

            return Ok(OrderDraftDto.FromOrder(order));
        }
    }
}
