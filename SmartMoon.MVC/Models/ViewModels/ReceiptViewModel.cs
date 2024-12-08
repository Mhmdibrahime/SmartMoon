using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class ReceiptViewModel
    {
        public int Id { get; set; }
        public int MoneyDrawerId { get; set; }
        public string? ClientName { get; set; }
       
        public List<MoneyDrawer>? moneyDrawers { get; set; }  
        public decimal PreviousBalance { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal NewBalance => PreviousBalance - PaymentAmount;
    }

}
