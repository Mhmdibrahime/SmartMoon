using System.ComponentModel.DataAnnotations;

namespace SmartMoon.MVC.Models.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(450)]
        public string Address { get; set; }
        [MaxLength (255)]
        public string? FirstRepresentativeName { get; set; }
        [MaxLength (15)]
        public string? FirstRepresentativePhoneNumber { get; set; }
        [MaxLength(255)]
        public string? SecondRepresentativeName { get; set; }
        [MaxLength(15)]
        public string? SecondRepresentativePhoneNumber { get; set; }
        public decimal Balance { get; set; } = 0;
        public ICollection<ProductSupplier> productSuppliers { get; set;}
        public ICollection<BuyBill> Bills { get; set; }

    }
}
