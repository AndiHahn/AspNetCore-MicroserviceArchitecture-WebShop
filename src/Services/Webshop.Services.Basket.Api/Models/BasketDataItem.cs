namespace Webshop.Services.Basket.Api.Models
{
    public class BasketDataItem
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUri { get; set; }
    }
}
