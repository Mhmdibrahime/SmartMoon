namespace SmartMoon.MVC.Models.Entities
{
    public class BuyBill
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CashPaid { get; set; } 
        public decimal RemainingBalance { get; set; } 
        public string PaymentMethod { get; set; } 
        public string MoneyDrawer { get; set; }
       
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<BuyBillItem> BillItems { get; set; } 
        
        
    }
}
