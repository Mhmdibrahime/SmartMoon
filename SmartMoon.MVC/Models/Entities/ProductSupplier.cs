namespace SmartMoon.MVC.Models.Entities
{
    public class ProductSupplier
    {
        public Supplier Supplier { get; set; }
        public int SupplierId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
