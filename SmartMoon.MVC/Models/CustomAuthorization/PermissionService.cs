using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SmartMoon.MVC.Models.Data;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.CustomAuthorization
{
    public class PermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;

        public PermissionService(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<bool> HasPermissionAsync(string userId, string permission)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("مدير"))
                return true; 

            
            return  _dbContext.permissions
                                    .Any(up => up.UserId == userId && up.Permission == permission && up.IsGranted);
        }
    }
    public class PermissionFilter : IAsyncActionFilter
    {
        private readonly PermissionService _permissionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _permission;

        public PermissionFilter(PermissionService permissionService, UserManager<ApplicationUser> userManager, string permission)
        {
            _permissionService = permissionService;
            _userManager = userManager;
            _permission = permission;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }
            var userId = _userManager.GetUserId(context.HttpContext.User);
            var hasPermission = await _permissionService.HasPermissionAsync(userId, _permission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult(); 
                return;
            }

            await next(); 
        }
    }
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(string permission) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }
}
