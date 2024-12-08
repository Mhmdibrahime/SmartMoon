namespace SmartMoon.MVC.Models.Entities
{
    public class SalesBillItem
    {
        public int Id { get; set; }
        public int SalesBillId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int InventoryId { get; set; } // Inventory for this item
        public Inventory Inventory { get; set; }
        // Navigation properties
        public SalesBill SalesBill { get; set; }
        public Product Product { get; set; }
    }
}
