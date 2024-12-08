namespace SmartMoon.MVC.Models.ViewModels
{
    public class BillItemViewModel
    {
        
        public int ProductId { get; set; } // Product being purchased
       
        public int Quantity { get; set; } // Quantity of the product being purchased
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Total => PurchasePrice * Quantity; // Calculated total (Price * Quantity)
        public int InventoryId { get; set; } // Selected inventory for the product
        
    }

}
