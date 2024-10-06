using System.ComponentModel.DataAnnotations;

namespace SmartMoon.MVC.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(450)]
        public string Name { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        
        public ICollection<ProductSupplier> productSuppliers { get; set;} 

    }
}
