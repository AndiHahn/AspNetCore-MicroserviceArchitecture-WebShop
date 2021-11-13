namespace Webshop.WebApps.MvcClient.ViewModels.BasketViewModels
{
    public class BasketComponentViewModel
    {
        public int ItemsCount { get; set; }
        public string Disabled => (ItemsCount == 0) ? "is-disabled" : string.Empty;
    }
}
