using System;
using Webshop.CrossCutting.MessageBus.Models;
using Webshop.Services.Basket.Api.Models;

namespace Webshop.Services.Basket.Api.Events
{
    public class BasketCheckoutIntegrationEvent : IntegrationEvent
    {
        public string CustomerId { get; set; }
        public string Buyer { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardSecurityNumber { get; set; }
        public DateTime CreditCardExpiration { get; set; }
        public BasketData Basket { get; }

        public BasketCheckoutIntegrationEvent(
            string customerId,
            string buyer,
            string zipCode,
            string city,
            string street,
            string country,
            string creditCardHolder,
            string creditCardNumber,
            string creditCartSecurityNumber,
            DateTime creditCartExpiration,
            BasketData basket)
        {
            this.CustomerId = customerId;
            this.Buyer = buyer;
            this.ZipCode = zipCode;
            this.City = city;
            this.Street = street;
            this.Country = country;
            this.CreditCardHolder = creditCardHolder;
            this.CreditCardNumber = creditCardNumber;
            this.CreditCardSecurityNumber = creditCartSecurityNumber;
            this.CreditCardExpiration = creditCartExpiration;
            this.Basket = basket;
        }
    }
}
