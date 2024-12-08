namespace SmartMoon.MVC.Models.ViewModels
{
    public class SalesBillVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string SalesPoint { get; set; }
        public string SalesPerson { get; set; }
    }

}
