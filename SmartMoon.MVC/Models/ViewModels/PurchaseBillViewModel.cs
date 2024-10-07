using Microsoft.AspNetCore.Mvc.Rendering;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class PurchaseBillViewModel
    {
        public Supplier Supplier { get; set; }
        public int SupplierId { get; set; } // Selected Supplier ID
        public List<Supplier> Suppliers { get; set; } // Dropdown list of suppliers
        public List<Product> Products { get; set; }
        public List<BillItemViewModel> Items { get; set; } // List of items in the bill

        public Inventory Inventory { get; set; }
        public int InventoryId { get; set; } // Store/Inventory ID
        public List<Inventory> Inventories { get; set; } // Dropdown list of stores

        public decimal TotalAmount { get; set; } // Total amount
        public decimal DiscountAmount { get; set; } // Discount amount

        public string PaymentMethod { get; set; } // Payment method selected
        public decimal CashPaid { get; set; } // Cash amount paid
        public decimal RemainingBalance { get; set; } // Remaining balance after payment
        public string MoneyDrawer {  get; set; }
        public List<MoneyDrawer> MoneyDrawers { get; set; }
    }
}
