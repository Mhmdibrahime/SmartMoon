namespace SmartMoon.MVC.Models.ViewModels
{
    public class ProductUpdateViewModel
    {
        public int ProductId { get; set; }
        public int BatchId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }

    }

}
