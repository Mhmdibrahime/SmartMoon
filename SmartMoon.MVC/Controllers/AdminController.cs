using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartMoon.MVC.Models.Data;
using SmartMoon.MVC.Models.Entities;
using SmartMoon.MVC.Models.ViewModels;

namespace SmartMoon.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext context;

        public AdminController(AppDbContext context)
        {
            this.context = context;
        }

        public ActionResult AddClient() 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]  
        public IActionResult AddClient(NewClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = new Client()
                    {
                        Name = model.Name,
                        Address = model.Address,
                        NationalID = model.NationalID,
                        PhoneNumber = model.PhoneNumber,
                        MobileNumber = model.MobileNumber,
                    };
                    context.clients.Add(client);
                    context.SaveChanges();
                    return RedirectToAction("Index","Home");
                }
                catch (Exception ex)
                {
                    throw new Exception("!بيانات العميل المدخل ليست صحيحة");
                }
            }
            return View(model);
        }

        public IActionResult AddSupplier()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSupplier(NewSupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var supplier = new Supplier()
                    {
                        Name = model.Name,
                        Address = model.Address,
                        FirstRepresentativeName = model.FirstRepresentativeName,
                        FirstRepresentativePhoneNumber = model.FirstRepresentativePhoneNumber,
                        SecondRepresentativeName = model.SecondRepresentativeName,
                        SecondRepresentativePhoneNumber =model.SecondRepresentativePhoneNumber
                    };
                    context.suppliers.Add(supplier);
                    context.SaveChanges();
                    return View();
                }
                catch
                {
                    throw new Exception("!بيانات المورد المدخله ليست صالحه");
                }  
            }
            return View(model);
        }

        public IActionResult ViewProducts()
        {
            var productsWithSuppliers = context.products.
                Include(x => x.productSuppliers).
                ThenInclude(x => x.Supplier).ToList();
            var products = productsWithSuppliers.Select(p => new ProductsViewModel
            {
               
                ProductName = p.Name,
                Price = p.Price,
                Quantity = p.Quantity,
                SuppliersName = p.productSuppliers.Select(ps=>ps.Supplier.Name).ToList()
            }).ToList();
            var model = new ViewProductsWithSuppliersViewModel
            {
                Products = products,
                Suppliers = context.suppliers.ToList()
            };
            return View(model);
        }

        public IActionResult AddProduct(ViewProductsWithSuppliersViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = new Product
                    {
                        Name = model.ProductName,
                    };
                    context.products.Add(product);
                    context.SaveChanges();
                }
                catch
                {
                    throw new Exception();
                }
            }
            return RedirectToAction("ViewProducts");
        }
        public IActionResult CreatePurchaseBill()
        {
            var inv = context.inventories.ToList();
            var model = new PurchaseBillViewModel()
            {
                Suppliers = context.suppliers.ToList(),
                
                Inventories = inv,
                Products = context.products.ToList(),
                MoneyDrawers = context.moneyDrawer.ToList(),
                Items = new List<BillItemViewModel>()
            };

            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CreatePurchaseBill(PurchaseBillViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.TotalAmount = model.Items.Sum(item => item.PurchasePrice * item.Quantity);

                // Calculate Remaining Balance
                model.RemainingBalance = model.TotalAmount - model.DiscountAmount - model.CashPaid;

                // Create Bill entity
                var bill = new BuyBill
                {
                    SupplierId = model.SupplierId,
                    PaymentMethod = model.PaymentMethod,
                    DiscountAmount = model.DiscountAmount,
                    CashPaid = model.CashPaid,
                    RemainingBalance = model.RemainingBalance,
                    MoneyDrawer = model.MoneyDrawer,
                    TotalAmount = model.TotalAmount,
                    Date = DateTime.Now,
                    BillItems = model.Items.Select(item => new BillItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PurchasePrice = item.PurchasePrice,
                        SalePrice = item.SalePrice,
                        InventoryId = item.InventoryId
                    }).ToList()
                };

                context.buyBill.Add(bill);
                context.SaveChanges();

                // Redirect to a summary or details page
                return RedirectToAction("BillSummary", new { id = bill.Id });
            }

            // If model state is invalid, reload dropdowns
            model.Suppliers = context.suppliers.ToList();

            model.Products = context.products.ToList();
            model.Inventories = context.inventories.ToList();

            return View(model);
        }

        public IActionResult BillSummary(int id)
        {
            var bill = context.buyBill
                .Include(b => b.Supplier)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Product)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Inventory)
                .FirstOrDefault(b => b.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }
    }
}
