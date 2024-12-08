namespace SmartMoon.MVC.Models.Entities
{
    public class MoneyDrawer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal CurrentBalance {  get; set; }
        public ICollection<Expense>? Expenses { get; set; }
    }
}
