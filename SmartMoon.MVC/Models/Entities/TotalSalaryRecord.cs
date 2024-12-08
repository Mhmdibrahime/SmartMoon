namespace SmartMoon.MVC.Models.Entities
{
    public class TotalSalaryRecord
    {
        public int Id { get; set; }
        public string MoneyDrawer { get; set; }
        public decimal TotalNetSalary { get; set; }
        public DateTime Date { get; set; }

        public ApplicationUser? User { get; set; }
        public string? UserId { get; set; }
    }

}
