namespace SmartMoon.MVC.Models.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public int MoneyDrawerId { get; set; }
        public MoneyDrawer? MoneyDrawer { get; set; }
        public DateTime? ExpenseDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}

