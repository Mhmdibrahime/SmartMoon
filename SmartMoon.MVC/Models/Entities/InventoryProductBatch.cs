namespace SmartMoon.MVC.Models.Entities
{
    public class InventoryProductBatch
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public int ProductBatchId { get; set; }
        public ProductBatch ProductBatch { get; set; }

        public int Quantity { get; set; } // Quantity available from this batch in this inventory
    }
}