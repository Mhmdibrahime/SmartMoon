namespace SmartMoon.MVC.Models.Entities
{
    public class NetEmpSalary
    {
        public int Id { get; set; }
        public Employee? Employee { get; set; }
        public int EmployeeId { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public string? MoneyDrawer { get; set; }

        public DateTime Date { get; set; }
    }
}
