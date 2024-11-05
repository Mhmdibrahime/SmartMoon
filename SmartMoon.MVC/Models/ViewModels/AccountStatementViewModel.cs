using System.Numerics;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class AccountStatementViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        public List<ReceiptVM> Receipts { get; set; }
        public List<SalesBillVM> SalesBills { get; set; }
        public List<PurchaseBillVM> PurchaseBills { get; set; }

        public decimal TotalCashpaid {  get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalRemaining { get; set; }
    }


}
