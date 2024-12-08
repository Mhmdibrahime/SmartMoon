using Microsoft.AspNetCore.Authorization;
using SmartMoon.MVC.Models.Data;
using System.Security.Claims;

namespace SmartMoon.MVC.Models.CustomAuthorization
{
    //public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    //{
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public PermissionHandler(IHttpContextAccessor httpContextAccessor)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    protected override async Task HandleRequirementAsync(
    //        AuthorizationHandlerContext context, PermissionRequirement requirement)
    //    {
    //        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

    //        if (context.User.IsInRole("مدير"))
    //        {
                
    //            context.Succeed(requirement);
    //            return;
    //        }

    //        if (!string.IsNullOrEmpty(userId))
    //        {
                
    //            var dbContext = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
    //            var hasPermission = dbContext.permissions
    //                .Any(p => p.UserId == userId && p.Permission == requirement.Permission && p.IsGranted);

    //            if (hasPermission)
    //            {
    //                context.Succeed(requirement);
    //                return;
    //            }
    //        }

    //        context.Fail();
    //    }
    //}

}
