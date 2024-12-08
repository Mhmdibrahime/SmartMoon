namespace SmartMoon.MVC.Models.Entities
{
    public class BuyBillItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Total { get; set; } // Price * Quantity

        public int InventoryId { get; set; } // Inventory for this item
        public Inventory Inventory { get; set; }
    }
}
