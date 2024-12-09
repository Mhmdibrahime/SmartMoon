using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class ProductsViewModel
    {
        public int ProductId { get; set; }
        public int BatchId { get; set; }
        [ValidateNever]
        public string ProductName { get; set; }
        [ValidateNever]
        public decimal PurchasePrice { get; set; }
        public decimal Price { get; set; }
        [ValidateNever]
        public string SupplierName { get; set; }
        [ValidateNever]
        public string InventoryName { get; set; } 
        public int InventoryId { get; set; } 
        [ValidateNever]
        public int Quantity { get; set; }




    }
}
