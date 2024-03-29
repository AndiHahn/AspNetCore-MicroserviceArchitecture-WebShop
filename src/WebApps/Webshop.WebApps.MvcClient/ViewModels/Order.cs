﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Webshop.WebApps.MvcClient.ViewModels.Annotations;

namespace Webshop.WebApps.MvcClient.ViewModels
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }

        public string Description { get; set; }

        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string ZipCode { get; set; }

        [Required]
        [DisplayName("Card number")]
        public string CardNumber { get; set; }
        [Required]
        [DisplayName("Cardholder name")]
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
        [CardExpiration(ErrorMessage = "The card is expired"), Required]
        [DisplayName("Card expiration")]
        public string CardExpirationShort { get; set; }
        [Required]
        [DisplayName("Card security number")]
        public string CardSecurityNumber { get; set; }

        public string Buyer { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public void CardExpirationShortFormat()
        {
            CardExpirationShort = CardExpiration.ToString("MM/yy");
        }

        public void CardExpirationApiFormat()
        {
            var month = CardExpirationShort.Split('/')[0];
            var year = $"20{CardExpirationShort.Split('/')[1]}";

            CardExpiration = new DateTime(int.Parse(year), int.Parse(month), 1);
        }
    }
}
