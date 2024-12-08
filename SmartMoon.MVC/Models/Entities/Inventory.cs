namespace SmartMoon.MVC.Models.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<InventoryProduct>? inventoryProducts { get; set; }
        
    }
}
