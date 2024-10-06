using Microsoft.AspNetCore.Mvc;
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
    }
}
