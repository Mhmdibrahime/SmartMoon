using System.ComponentModel.DataAnnotations;

namespace SmartMoon.MVC.Models.Entities
{
    public class Client
    {

        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(450)]
        public string Address { get; set; }

        [MaxLength(14)]
        public string NationalID { get; set; }
        [MaxLength(11)]
        public string? MobileNumber { get; set;}
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        public decimal Balance { get; set; } = 0;

    }
}
