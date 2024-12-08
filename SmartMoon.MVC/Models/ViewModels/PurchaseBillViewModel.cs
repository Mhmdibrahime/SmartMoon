using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class PurchaseBillViewModel
    {
        public PurchaseBillViewModel()
        {
            Suppliers = new List<Supplier>();
            Products = new List<Product>(); 
            Inventories = new List<Inventory>();
            MoneyDrawers = new List<MoneyDrawer>();
        }
        [ValidateNever]
        public Supplier Supplier { get; set; }
        public int SupplierId { get; set; } // Selected Supplier ID
        [ValidateNever]
        public List<Supplier> Suppliers { get; set; }
        [ValidateNever]
        public List<Product> Products { get; set; }
        
        public List<BillItemViewModel> Items { get; set; } // List of items in the bill

       
        [ValidateNever]
        public List<Inventory> Inventories { get; set; } // Dropdown list of stores

        public decimal TotalAmount { get; set; } // Total amount
        public decimal DiscountAmount { get; set; } // Discount amount
        public string DiscountMethod { get; set; } 

        public string PaymentMethod { get; set; } // Payment method selected
        public decimal CashPaid { get; set; } // Cash amount paid
        public decimal RemainingBalance { get; set; } // Remaining balance after payment
        public string MoneyDrawer {  get; set; }
        [ValidateNever]
        public List<MoneyDrawer> MoneyDrawers { get; set; } 
    }
}
