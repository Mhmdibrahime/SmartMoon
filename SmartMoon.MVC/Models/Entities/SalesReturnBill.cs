namespace SmartMoon.MVC.Models.Entities
{
    public class SalesReturnBill
    {
        public int Id { get; set; }

        public int ClientId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CashPaid { get; set; }
        public decimal RemainingBalance { get; set; }
        public string MoneyDrawer { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime Date { get; set; }

        // Navigation property for related entities
        public Client client { get; set; }
        public ICollection<SalesReturnBillItem> Items { get; set; }
    }
}
