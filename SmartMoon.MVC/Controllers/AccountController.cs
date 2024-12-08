using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMoon.MVC.Models.Data;
using SmartMoon.MVC.Models.Entities;
using SmartMoon.MVC.Models.ViewModels;
using System.Reflection.Metadata.Ecma335;

namespace SmartMoon.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(AppDbContext context,
            UserManager<ApplicationUser> userManager ,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [Authorize(Roles = "مدير")]
        [HttpGet]
        public IActionResult AddUser()
        {
            var model = new AddUserViewModel
            {
                AllPermissions = new List<string>
        {
            "إضافة عميل", // AddClient
            "إضافة مورد", // AddSupplier
            "عرض المنتجات", // ViewProducts
            "إضافة منتج", // AddProduct
            "حذف منتج", // DeleteProduct
            "إنشاء فاتورة شراء", // CreatePurchaseBill
            "إنشاء فاتورة مبيعات", // CreateSalesBill
            "إنشاء فاتورة مردود شراء", // CreatePurchaseReturnBill
            "إنشاء فاتورة مردود مبيعات", // CreateReturnSalesBill
            "إضافة مصروف", // AddExpense
            "التحويل بين الخزنات", // TransferBetweenMoneyDrawers
            "إضافة خزنة جديدة", // AddNewMoneyDrawer
            "التحويل بين المخازن", // TransferBetweenInventories
            "إضافة مخزون جديد", // AddNewInventory
            "إضافة إيصال من العميل", // AddFromClientReceipt
            "إضافة إيصال للعميل", // AddToClientReceipt
            "عرض العملاء", // ViewClients
            "كشف حساب العميل", // AccountStatement
            "حذف عميل", // DeleteClient
            "إضافة إيصال من المورد", // AddFromSupplierReceipt
            "إضافة إيصال للمورد", // AddToSupplierReceipt
            "عرض الموردين", // ViewSuppliers
            "حذف مورد", // DeleteSupplier
            "كشف حساب المورد", // SupplierAccountStatement
            "قائمة الأسعار", // PriceList
            "عرض نقص الأصناف", // ViewProductsShortcomings
            "أقساط متأخرة", // LateInstallments
            "الموقف المالي", // FinancialPosition
            "حركة الأصناف", // ItemMovement
            "عرض المصروفات اليومية", // ViewDailyExpenses
            "عرض المبيعات اليومية", // ViewDailySales
            "عرض عمليات الخزنة", // ViewDrawerOperations
            "عرض صافي الربح", // ViewNetProfit
            "عرض الموظفين", // ViewEmployees
            "إضافة موظف", // AddEmployee
            "حذف موظف", // DeleteEmployee
            "عرض السلف والحوافز والخصومات", // ViewAdvancesIncentivesAndDiscounts
            "صرف رواتب الموظفين" // PayingSalaries
        }
            };

            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {

           
            //var rolesToCheck = new[] { "مدير", "مستخدم عادي" };

            //foreach (var role in rolesToCheck)
            //{
               
            //    if (!await _roleManager.RoleExistsAsync(role))
            //    {
            //        await _roleManager.CreateAsync(new IdentityRole(role));
            //    }
            //}

            if (!ModelState.IsValid) {
                model.AllPermissions = new List<string>
        {
            "إضافة عميل", // AddClient
            "إضافة مورد", // AddSupplier
            "عرض المنتجات", // ViewProducts
            "إضافة منتج", // AddProduct
            "حذف منتج", // DeleteProduct
            "إنشاء فاتورة شراء", // CreatePurchaseBill
            "إنشاء فاتورة مبيعات", // CreateSalesBill
            "إنشاء فاتورة مردود شراء", // CreatePurchaseReturnBill
            "إنشاء فاتورة مردود مبيعات", // CreateReturnSalesBill
            "إضافة مصروف", // AddExpense
            "التحويل بين الخزنات", // TransferBetweenMoneyDrawers
            "إضافة خزنة جديدة", // AddNewMoneyDrawer
            "التحويل بين المخازن", // TransferBetweenInventories
            "إضافة مخزون جديد", // AddNewInventory
            "إضافة إيصال من العميل", // AddFromClientReceipt
            "إضافة إيصال للعميل", // AddToClientReceipt
            "عرض العملاء", // ViewClients
            "كشف حساب العميل", // AccountStatement
            "حذف عميل", // DeleteClient
            "إضافة إيصال من المورد", // AddFromSupplierReceipt
            "إضافة إيصال للمورد", // AddToSupplierReceipt
            "عرض الموردين", // ViewSuppliers
            "حذف مورد", // DeleteSupplier
            "كشف حساب المورد", // SupplierAccountStatement
            "قائمة الأسعار", // PriceList
            "عرض نقص الأصناف", // ViewProductsShortcomings
            "أقساط متأخرة", // LateInstallments
            "الموقف المالي", // FinancialPosition
            "حركة الأصناف", // ItemMovement
            "عرض المصروفات اليومية", // ViewDailyExpenses
            "عرض المبيعات اليومية", // ViewDailySales
            "عرض عمليات الخزنة", // ViewDrawerOperations
            "عرض صافي الربح", // ViewNetProfit
            "عرض الموظفين", // ViewEmployees
            "إضافة موظف", // AddEmployee
            "حذف موظف", // DeleteEmployee
            "عرض السلف والحوافز والخصومات", // ViewAdvancesIncentivesAndDiscounts
            "صرف رواتب الموظفين" // PayingSalaries
        };
                return View(model); }

            var user = new ApplicationUser { UserName = model.UserName };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                if (model.Role == "مستخدم عادي")
                {
                    
                    foreach (var permission in model.Permissions)
                    {
                        var userPermission = new UserPermission
                        {
                            UserId = user.Id,
                            Permission = permission,
                            IsGranted = true
                        };
                        _context.permissions.Add(userPermission);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Home");
            }

            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            model.AllPermissions = new List<string>
{
    "إضافة عميل", // AddClient
    "إضافة مورد", // AddSupplier
    "عرض المنتجات", // ViewProducts
    "إضافة منتج", // AddProduct
    "حذف منتج", // DeleteProduct
    "إنشاء فاتورة شراء", // CreatePurchaseBill
    "إنشاء فاتورة مبيعات", // CreateSalesBill
    "إنشاء فاتورة مردود شراء", // CreatePurchaseReturnBill
    "إنشاء فاتورة مردود مبيعات", // CreateReturnSalesBill
    "إضافة مصروف", // AddExpense
    "التحويل بين الخزنات", // TransferBetweenMoneyDrawers
    "إضافة خزنة جديدة", // AddNewMoneyDrawer
    "التحويل بين المخازن", // TransferBetweenInventories
    "إضافة مخزون جديد", // AddNewInventory
    "إضافة إيصال من العميل", // AddFromClientReceipt
    "إضافة إيصال للعميل", // AddToClientReceipt
    "عرض العملاء", // ViewClients
    "كشف حساب العميل", // AccountStatement
    "حذف عميل", // DeleteClient
    "إضافة إيصال من المورد", // AddFromSupplierReceipt
    "إضافة إيصال للمورد", // AddToSupplierReceipt
    "عرض الموردين", // ViewSuppliers
    "حذف مورد", // DeleteSupplier
    "كشف حساب المورد", // SupplierAccountStatement
    "قائمة الأسعار", // PriceList
    "عرض نقص الأصناف", // ViewProductsShortcomings
    "أقساط متأخرة", // LateInstallments
    "الموقف المالي", // FinancialPosition
    "حركة الأصناف", // ItemMovement
    "عرض المصروفات اليومية", // ViewDailyExpenses
    "عرض المبيعات اليومية", // ViewDailySales
    "عرض عمليات الخزنة", // ViewDrawerOperations
    "عرض صافي الربح", // ViewNetProfit
    "عرض الموظفين", // ViewEmployees
    "إضافة موظف", // AddEmployee
    "حذف موظف", // DeleteEmployee
    "عرض السلف والحوافز والخصومات", // ViewAdvancesIncentivesAndDiscounts
    "صرف رواتب الموظفين" // PayingSalaries
};
            return View(model);
        }



        [HttpGet]
        public IActionResult Login()
        {
            //if (_userManager.Users.Count() == 0) return RedirectToAction("AddNewAdmin");
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }

            
            return View(model);
        }

       
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult AddNewAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewAdmin(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var rolesToCheck = new[] { "مدير", "مستخدم عادي" };

            foreach (var role in rolesToCheck)
            {

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            

            var user = new ApplicationUser { UserName = model.UserName };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "مدير");

               

                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}
