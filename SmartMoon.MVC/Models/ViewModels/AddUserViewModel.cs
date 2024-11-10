namespace SmartMoon.MVC.Models.ViewModels
{
    public class AddUserViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; } 
        public List<string>? Permissions { get; set; } 
        public List<string>? AllPermissions { get; set; } 
    }

}
