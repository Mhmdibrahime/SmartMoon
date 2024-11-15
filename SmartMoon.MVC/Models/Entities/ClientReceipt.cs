namespace SmartMoon.MVC.Models.Entities
{
    public class ClientReceipt
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Type { get; set; }

        public string? MoneyDrawer { get; set; }
        public virtual Client? Client { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }

}
