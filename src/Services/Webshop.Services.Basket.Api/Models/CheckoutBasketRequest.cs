using System;

namespace Webshop.Services.Basket.Api.Models
{
    public class CheckoutBasketRequest
    {
        public string Buyer { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardSecurityNumber { get; set; }
        public DateTime CreditCardExpiration { get; set; }
    }
}
