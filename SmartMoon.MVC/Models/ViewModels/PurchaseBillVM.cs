namespace SmartMoon.MVC.Models.ViewModels
{
    public class PurchaseBillVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CashPaid { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string SalesPoint { get; set; }
        public string SalesPerson { get; set; }
    }

}
