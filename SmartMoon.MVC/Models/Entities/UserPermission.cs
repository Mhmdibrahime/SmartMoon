namespace SmartMoon.MVC.Models.Entities
{
    public class UserPermission
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string Permission { get; set; } 
        public bool IsGranted { get; set; }

        public virtual ApplicationUser? User { get; set; } 
    }

}
