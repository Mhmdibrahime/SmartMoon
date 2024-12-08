namespace SmartMoon.MVC.Models.ViewModels
{
    public class PayingSalariesViewModel
    {
        public List<EmployeeSalaryViewModel> Employees { get; set; }
        public List<string> MoneyDrawers { get; set; }
        public string SelectedMoneyDrawer { get; set; }
    }
}
