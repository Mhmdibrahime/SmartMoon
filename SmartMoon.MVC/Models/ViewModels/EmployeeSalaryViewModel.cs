namespace SmartMoon.MVC.Models.ViewModels
{
    public class EmployeeSalaryViewModel
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Incentive { get; set; }
        public decimal Deduction { get; set; }
        public decimal Advance { get; set; }
        public decimal NetSalary { get; set; }

        public decimal TotalSales { get; set; }
    }

}
