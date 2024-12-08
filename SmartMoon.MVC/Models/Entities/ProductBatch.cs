namespace SmartMoon.MVC.Models.Entities
{
    public class ProductBatch
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public Inventory Inventory { get; set; }
        public int InventoryId { get; set; }
        public decimal PurchasePrice { get; set; } // Price at the time of purchase
        public int Quantity { get; set; }          // Quantity in this batch

        public DateTime PurchaseDate { get; set; } // Date of purchase for FIFO/LIFO
    }
}