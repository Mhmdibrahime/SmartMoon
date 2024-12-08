using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartMoon.MVC.Models.Data;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.DbIntializer
{
    public class DBIntializer : IDBIntializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext context;

        public DBIntializer(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,AppDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            this.context = context;
        }

        public void Initialize()
        {
            if(context.Database.GetPendingMigrations().Count()>0)
            {
                context.Database.Migrate();
            }

            if (!_roleManager.RoleExistsAsync("مدير").Result)
            {
                _roleManager.CreateAsync(new IdentityRole("مدير")).Wait();
                _roleManager.CreateAsync(new IdentityRole("مستخدم عادي")).Wait();
            }


            var adminUser = _userManager.FindByNameAsync("سامي").Result;
            if (adminUser == null)
            {
                var defaultUser = new ApplicationUser
                {
                    UserName = "سامي",
                    
                };

                var result = _userManager.CreateAsync(defaultUser, "147852").Result;
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(defaultUser, "مدير").Wait();
                }
            }
        }
    }
}
