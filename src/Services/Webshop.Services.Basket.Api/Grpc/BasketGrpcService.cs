using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Webshop.Services.Basket.Api.Infrastructure.Repositories;
using Webshop.Services.Basket.Api.Models;
using static Basket;

namespace Webshop.Services.Basket.Api.Grpc
{
    public class BasketGrpcService : BasketBase
    {
        private readonly IBasketRepository repository;

        public BasketGrpcService(
            IBasketRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public override async Task<BasketResponse> GetBasket(
            GetBasketRequest request,
            ServerCallContext context)
        {
            var customers = repository.GetAllCustomers();
            if (!customers.Any())
            {
                return new BasketResponse
                {
                    CustomerId = Guid.NewGuid().ToString()
                };
            }

            var basket = await repository.GetBasketAsync(customers.First());

            return MapModelToResponse(basket);
        }

        public override async Task<BasketResponse> UpdateBasket(
            UpdateBasketRequest request,
            ServerCallContext context)
        {
            var basket = MapRequestToModel(request);

            var updatedBasket = await repository.UpdateBasketAsync(basket);
            if (updatedBasket != null)
            {
                return MapModelToResponse(updatedBasket);
            }

            context.Status = new Status(StatusCode.NotFound, $"Basket for customer {request.CustomerId} not found");
            return null;
        }

        private BasketResponse MapModelToResponse(BasketData basketData)
        {
            var response = new BasketResponse
            {
                CustomerId = basketData.CustomerId
            };

            foreach (var basketItem in basketData.Items)
            {
                response.Items.Add(new BasketItem
                {
                    Id = basketItem.Id,
                    ProductId = basketItem.ProductId,
                    ProductName = basketItem.ProductName,
                    UnitPrice = basketItem.UnitPrice,
                    Quantity = basketItem.Quantity,
                    PictureUri = basketItem.PictureUri
                });
            }

            return response;
        }

        private BasketData MapRequestToModel(UpdateBasketRequest request)
        {
            var model = new BasketData
            {
                CustomerId = request.CustomerId
            };

            foreach (var item in request.Items)
            {
                model.Items.Add(new BasketDataItem
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    PictureUri = item.PictureUri
                });
            }

            return model;
        }
    }
}
