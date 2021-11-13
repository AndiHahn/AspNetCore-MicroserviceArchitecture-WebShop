using System;
using Webshop.CrossCutting.MessageBus.Models;
using Webshop.Services.Order.Api.Application.Models;

namespace Webshop.Services.Order.Api.Application.Events
{
    public class BasketCheckoutIntegrationEvent : IntegrationEvent
    {
        public Guid CustomerId { get; set; }
        public string Buyer { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardSecurityNumber { get; set; }
        public DateTime CreditCardExpiration { get; set; }
        public CustomerBasket Basket { get; }

        public BasketCheckoutIntegrationEvent(
            Guid customerId,
            string buyer,
            string zipCode,
            string city,
            string street,
            string country,
            string creditCardHolder,
            string creditCardNumber,
            string creditCartSecurityNumber,
            DateTime creditCartExpiration,
            CustomerBasket basket)
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
