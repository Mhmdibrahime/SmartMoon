using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class ProductsViewModel
    {
        [ValidateNever]
        public string? ProductName { get; set; }
        [ValidateNever]
        public decimal? Price { get; set; }
        [ValidateNever]
        public int? Quantity { get; set; }
        [ValidateNever]
        public List<string>? SuppliersName { get; set; }

       

    }
}
