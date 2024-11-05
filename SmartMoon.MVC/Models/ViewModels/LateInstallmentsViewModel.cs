namespace SmartMoon.MVC.Models.ViewModels
{
    public class LateInstallmentsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public int DaysLate { get; set; }
        

    }
}
