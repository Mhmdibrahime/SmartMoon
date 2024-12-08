namespace SmartMoon.MVC.Models.Entities
{
    public class TransferBetweenMoneyDrawers
    {
        public int Id { get; set; }
        public int FromMoneyDrawerId { get; set; }
        public int ToMoneyDrawerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public virtual MoneyDrawer FromMoneyDrawer { get; set; }
        public virtual MoneyDrawer ToMoneyDrawer { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
