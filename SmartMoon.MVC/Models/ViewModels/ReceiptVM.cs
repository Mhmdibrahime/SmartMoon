namespace SmartMoon.MVC.Models.ViewModels
{
    public class ReceiptVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal AmountPaid { get; set; }
        public string Type { get; set; }
    }

}
