using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SmartMoon.MVC.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class SalesBillViewModel
    {
        public int ClientId { get; set; }
        [ValidateNever]
        public List<Client> clients { get; set; }
        [ValidateNever]
        public List<Product> products { get; set; }
        [ValidateNever]
        public List<Inventory> inventories { get; set; }  

        [Required]
        public List<SalesBillItemViewModel> Items { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }
        [DataType(DataType.Currency)]
        public decimal DiscountAmount { get; set; }
        [DataType(DataType.Currency)]
        public decimal CashPaid { get; set; }
        [DataType(DataType.Currency)]
        public decimal RemainingBalance { get; set; }
        public string DiscountMethod { get; set; }
        public string PaymentMethod { get; set; }
        [ValidateNever]
        public List<MoneyDrawer> MoneyDrawers { get; set; }
        public string MoneyDrawer { get; set; }

        public List<Employee>? Employees { get; set; }  
        public int? EmployeeId { get; set; }
    }
}
