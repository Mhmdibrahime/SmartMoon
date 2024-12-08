using System.ComponentModel.DataAnnotations;

namespace SmartMoon.MVC.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }
    }
}
