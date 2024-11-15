namespace SmartMoon.MVC.Models.Entities
{
    public class SalesBill
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CashPaid { get; set; }
        public decimal RemainingBalance { get; set; }
        public string MoneyDrawer { get; set; }
        public DateTime Date { get; set; }

        public string? PaymentMethod { get; set; }
        public int? EmployeeId { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public Client Client { get; set; }
        public Employee Employee { get; set; }  
        public ICollection<SalesBillItem> Items { get; set; }
    }

}
