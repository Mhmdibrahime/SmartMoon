namespace SmartMoon.MVC.Models.Entities
{
    public class SalesReturnBillItem
    {
        public int Id { get; set; }
        public int SalesReturnBillId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int InventoryId { get; set; } // Inventory for this item
        public Inventory Inventory { get; set; }
        // Navigation properties
        public SalesReturnBill SalesBill { get; set; }
        public Product Product { get; set; }
    }
}
