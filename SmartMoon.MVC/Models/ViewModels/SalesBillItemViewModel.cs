using System.ComponentModel.DataAnnotations;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class SalesBillItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        [DataType(DataType.Currency)]
        public decimal SalePrice { get; set; }
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
    }
}
