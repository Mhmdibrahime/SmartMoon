using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public List<Employee>? Employees { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Job { get; set; }
        public decimal Salary { get; set; }
        public decimal SalesRatio { get; set; }

        public decimal? TotalSalaries { get; set; }
    }
}
