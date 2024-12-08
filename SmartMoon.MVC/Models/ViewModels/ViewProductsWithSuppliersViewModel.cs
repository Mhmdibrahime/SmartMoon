using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class ViewProductsWithSuppliersViewModel
    {

        public string ProductName { get; set; }
        [ValidateNever]
        public List<ProductsViewModel> Products { get; set; }
        [ValidateNever]
        public List<Supplier>? Suppliers { get; set; }

        public List<Inventory>? inventories { get; set; }
    }
}
