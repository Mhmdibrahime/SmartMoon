using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class TransferMoneyViewModel
    {
        
        public List<MoneyDrawer> moneyDrawers;
        [ValidateNever]
        public int FromId { get; set; }
        [ValidateNever]
        public int ToId { get; set; }
        [ValidateNever]
        public decimal TotalAmount { get; set; }

        public string? NewMoneyDarwer {  get; set; }
    }
}
