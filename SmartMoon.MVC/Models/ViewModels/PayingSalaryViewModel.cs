using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class PayingSalaryViewModel
    {
        public List<string>? Drawers { get; set; }
        public List<Employee>? employees { get; set; }
    }
}
