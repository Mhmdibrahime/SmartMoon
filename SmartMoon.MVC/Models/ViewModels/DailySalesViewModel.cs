namespace SmartMoon.MVC.Models.ViewModels
{
    public class DailySalesViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CashPaid { get; set; }
        public decimal RemainingBalance { get; set; }
        public string ClientName { get; set; }
        public string UserName { get; set; }
    }
}
