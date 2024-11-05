using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class ExpenseViewModel
    {
        [ValidateNever]
        public string Item { get; set; }
        
        public List<MoneyDrawer>? moneyDrawers { get; set; }
        
        public List<Expense>? Expenses { get; set; }
    }
}
