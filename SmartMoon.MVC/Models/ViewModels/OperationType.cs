namespace SmartMoon.MVC.Models.ViewModels
{
    public class OperationType
    {
        
            public string Date { get; set; }   // The date of the operation
            public string Time { get; set; }    // The time of the operation
            public decimal Revenue { get; set; }   // The income value for the operation (if applicable)
            public decimal Expense { get; set; }  // The expense value for the operation (if applicable)
            public string Description { get; set; }  // A description or details about the operation
            public string User { get; set; }      // The user who performed the operation
        
    }
}
