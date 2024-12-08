namespace SmartMoon.MVC.Models.ViewModels
{
    public class FinancialPositionViewModel
    {
        public decimal ForClients { get; set; }
        public decimal ForSuppliers { get; set; }
        public decimal OnClients { get; set; }
        public decimal OnSuppliers { get; set; }
        public decimal InventoriesActualBalance {  get; set; }
        public decimal DrawersActualBalance { get; set; }
        public decimal TotalForUs { get; set; } 
        public decimal TotalOnUs { get; set; } 
        public decimal ActualTotalBalance { get; set; }
        public decimal FinalPosition { get; set; }

    }
}
