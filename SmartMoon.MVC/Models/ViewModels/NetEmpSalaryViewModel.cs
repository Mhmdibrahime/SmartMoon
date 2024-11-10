using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class NetEmpSalaryViewModel
    {
        public List<NetEmpSalary>? empSalaries { get; set; }
        public List<string>? Darwers { get; set; }
        public int  EmployeeId { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public string? MoneyDrawer { get; set; }

        public DateTime Date { get; set; }
    }
}
