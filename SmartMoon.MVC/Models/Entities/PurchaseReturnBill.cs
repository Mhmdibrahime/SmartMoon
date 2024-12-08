namespace SmartMoon.MVC.Models.Entities
{
    public class PurchaseReturnBill
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CashPaid { get; set; } // Amount paid in cash
        public decimal RemainingBalance { get; set; } // Amount remaining to be paid
        public string PaymentMethod { get; set; } // Payment Method (Cash, Credit, etc.)
        public string MoneyDrawer { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<PurchaseReturnBillItem> BillItems { get; set; } // List of items in the bill

    }
}
