using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webshop.WebApps.MvcClient.ViewModels;

namespace Webshop.WebApps.MvcClient.Services
{
    public interface IOrderService
    {
        Task<IList<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderById(Guid id);
        Task<Order> GetOrderDraftAsync(IEnumerable<BasketItem> basketItems);
    }
}
