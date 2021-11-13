using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Webshop.CrossCutting.MessageBus;
using Webshop.Services.Basket.Api.Events;
using Webshop.Services.Basket.Api.Exceptions;
using Webshop.Services.Basket.Api.Infrastructure.Repositories;
using Webshop.Services.Basket.Api.Models;

namespace Webshop.Services.Basket.Api.Services
{
    public class BasketService : IBasketService
    {
        private readonly IMessageSender messageSender;
        private readonly IBasketRepository basketRepository;
        private readonly ILogger<BasketService> logger;

        public BasketService(
            IMessageSender messageSender,
            IBasketRepository basketRepository,
            ILogger<BasketService> logger)
        {
            this.messageSender = messageSender ?? throw new System.ArgumentNullException(nameof(messageSender));
            this.basketRepository = basketRepository ?? throw new System.ArgumentNullException(nameof(basketRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CheckoutBasketAsync(string customerId, CheckoutBasketRequest checkoutRequest)
        {
            var basket = await basketRepository.GetBasketAsync(customerId);
            if (basket == null)
            {
                throw new NotFoundException("Basket for customer {customerId} not found.");
            }

            PublishCheckoutEventMessage(customerId, checkoutRequest, basket);

            await basketRepository.DeleteBasketAsync(customerId);
        }

        private void PublishCheckoutEventMessage(
            string customerId,
            CheckoutBasketRequest checkoutRequest,
            BasketData basket)
        {
            var @event = new BasketCheckoutIntegrationEvent(
                customerId,
                checkoutRequest.Buyer,
                checkoutRequest.ZipCode,
                checkoutRequest.City,
                checkoutRequest.Street,
                checkoutRequest.Country,
                checkoutRequest.CreditCardHolder,
                checkoutRequest.CreditCardNumber,
                checkoutRequest.CreditCardSecurityNumber,
                checkoutRequest.CreditCardExpiration,
                basket);

            try
            {
                messageSender.Publish(@event);

                logger.LogInformation($"Published {nameof(BasketCheckoutIntegrationEvent)}.");
            }
            catch (Exception ex)
            {
                logger.LogError("Error publishing checkout event: {m}", ex.Message);

                throw;
            }
        }
    }
}
