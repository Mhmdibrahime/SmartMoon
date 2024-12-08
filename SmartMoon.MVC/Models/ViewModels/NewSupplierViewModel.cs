using System.ComponentModel.DataAnnotations;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class NewSupplierViewModel
    {
        public string Name { get; set; }
        [MaxLength(450)]
        public string Address { get; set; }
        [MaxLength(255)]
        public string? FirstRepresentativeName { get; set; }
        [MaxLength(15)]
        public string? FirstRepresentativePhoneNumber { get; set; }
        [MaxLength(255)]
        public string? SecondRepresentativeName { get; set; }
        [MaxLength(15)]
        public string? SecondRepresentativePhoneNumber { get; set; }
    }
}
