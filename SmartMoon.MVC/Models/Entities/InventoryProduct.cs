namespace SmartMoon.MVC.Models.Entities
{
    public class InventoryProduct
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }

        public Inventory Inventory { get; set; }
        public int InventoryId { get; set; }
        public int Quantity { get; set; } 
    }
}
