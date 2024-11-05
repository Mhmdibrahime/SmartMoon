namespace SmartMoon.MVC.Models.ViewModels
{
    public class TransferViewModel
    {
       
            public int FromInventoryId { get; set; }
            public int ToInventoryId { get; set; }
            public List<TransferRequest> Transfers { get; set; }
        }
    
}
