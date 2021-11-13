using System;
using System.ComponentModel.DataAnnotations;

namespace Webshop.WebApps.MvcClient.ViewModels.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CardExpiration : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            var monthString = value.ToString().Split('/')[0];
            var yearString = $"20{value.ToString().Split('/')[1]}";

            if ((int.TryParse(monthString, out var month)) &&
                (int.TryParse(yearString, out var year)))
            {
                DateTime d = new DateTime(year, month, 1);

                return d > DateTime.UtcNow;
            }

            return false;
        }
    }
}
