using System.ComponentModel.DataAnnotations;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class NewClientViewModel
    {
        
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(450)]
        public string Address { get; set; }

        [MaxLength(14)]
        public string NationalID { get; set; }
        [MaxLength(11)]
        public string? MobileNumber { get; set; }
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
    }
}
