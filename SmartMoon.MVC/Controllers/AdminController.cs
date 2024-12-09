using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using NuGet.Versioning;
using SmartMoon.MVC.Models.CustomAuthorization;
using SmartMoon.MVC.Models.Data;
using SmartMoon.MVC.Models.Entities;
using SmartMoon.MVC.Models.ViewModels;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace SmartMoon.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext context;

        public AdminController(AppDbContext context)
        {
            this.context = context;
        }

        
        [Permission("إضافة عميل")]
        public ActionResult AddClient() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        [Permission("إضافة عميل")]
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
        [Permission("إضافة مورد")]
        public IActionResult AddSupplier()
        {
            return View();
        }

        [Permission("إضافة مورد")]
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
                    return RedirectToAction("index", "home");
                }
                catch
                {
                    throw new Exception("!بيانات المورد المدخله ليست صالحه");
                }  
            }
            return View(model);
        }

        [Permission("عرض المنتجات")]
        public IActionResult ViewProducts()
        {
            var productInventories = context.products
                .Include(p => p.ProductBatches)
                    .ThenInclude(pb => pb.Inventory)
                .Include(p => p.productSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .Where(p => p.Quantity > 0)
                .SelectMany(p => p.ProductBatches.Where(x => x.Quantity > 0)
                    .Select(pb => new ProductsViewModel
                    {
                        ProductId = p.Id,
                        BatchId = pb.Id,
                        ProductName = p.Name,
                        Price = p.Price,
                        SupplierName = p.productSuppliers.Select(ps => ps.Supplier.Name).FirstOrDefault() ?? "غير معروف",
                        InventoryName = pb.Inventory.Name,
                        InventoryId = pb.Inventory.Id,
                        Quantity = pb.Quantity,
                        PurchasePrice = pb.PurchasePrice
                    }))
                .ToList();

            var model = new ViewProductsWithSuppliersViewModel
            {
                Products = productInventories,
                Suppliers = context.suppliers.ToList(),
                inventories = context.inventories.ToList()
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult GetInventoryBalance(int? inventoryId)
        {
            try
            {
                decimal balance = 0;

                if (inventoryId.HasValue && inventoryId > 0)
                {
                    // Calculate balance for the selected inventory
                    balance = context.productBatches
                        .Where(pb => pb.InventoryId == inventoryId.Value)
                        .Sum(pb => pb.Quantity * pb.PurchasePrice);
                }
                else
                {
                    // Calculate balance for all inventories
                    balance = context.productBatches
                        .Sum(pb => pb.Quantity * pb.PurchasePrice);
                }

                return Json(new { balance });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error calculating inventory balance.", error = ex.Message });
            }
        }




        [Permission("إضافة منتج")]
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

        [HttpPost]
        public IActionResult UpdateProduct([FromBody] ProductUpdateViewModel model)
        {
            var product = context.products.FirstOrDefault(p => p.Id == model.ProductId);
            if (product == null)
            {
                return NotFound(new { Message = "Product not found" });
            }
            var batch = context.productBatches.FirstOrDefault(x => x.Id == model.BatchId);
            if (batch == null)
            {
                return NotFound(new { Message = "Product not found" });
            }
            product.Name = model.ProductName;
            product.Price = model.Price;

            var otherBatches = context.productBatches.Any(x => x.ProductId == model.ProductId);
            if (otherBatches)
            {
                var newQuantity = model.Quantity;
                var productQuantity = product.Quantity;
                var batchesQuantiy = context.productBatches.Where(x => x.ProductId == model.ProductId).Where(x => x.Id != model.BatchId).Sum(x => x.Quantity);

                product.Quantity = newQuantity + batchesQuantiy;
                batch.Quantity = newQuantity;

                batch.PurchasePrice = model.PurchasePrice;

                //if(newQuantity+batchesQuantiy>productQuantity)
                //{
                //    batch.Quantity = newQuantity;

                //    product.Quantity = newQuantity + batchesQuantiy;
                //    batch.PurchasePrice = model.PurchasePrice;
                //}
                //else
                //{

                //}
            }
            else
            {
                batch.Quantity = model.Quantity;
                product.Quantity = model.Quantity;
                batch.PurchasePrice = model.PurchasePrice;
            }
            
            context.SaveChanges();

            return Ok();
        }


        [Permission("حذف منتج")]
        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var product = context.products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                var productBatches = context.productBatches.Where(pb => pb.ProductId == id).ToList();
                context.productBatches.RemoveRange(productBatches);

                var inventoryProducts = context.inventoryProducts.Where(ip => ip.ProductId == id).ToList();
                context.inventoryProducts.RemoveRange(inventoryProducts);

                context.products.Remove(product);
                context.SaveChanges();
            }

            return RedirectToAction("ViewProductsShortcomings");
        }


        [Permission("إنشاء فاتورة شراء")]
        public IActionResult CreatePurchaseBill()
        {
            var model = new PurchaseBillViewModel
            {
                Suppliers = context.suppliers.ToList(),
                Inventories = context.inventories.ToList(),
                Products = context.products.ToList(),
                MoneyDrawers = context.moneyDrawer.ToList(),
                Items = new List<BillItemViewModel>()
            };

            return View(model);
        }


        [Permission("إنشاء فاتورة شراء")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CreatePurchaseBill(PurchaseBillViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.DiscountMethod == "Percentage") model.DiscountAmount = (model.DiscountAmount * model.TotalAmount) / 100;
                model.TotalAmount = model.Items.Sum(item => item.PurchasePrice * item.Quantity);
                model.RemainingBalance = model.TotalAmount - model.DiscountAmount - model.CashPaid;
                var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var bill = new BuyBill
                {
                    SupplierId = model.SupplierId,
                    PaymentMethod = model.PaymentMethod,
                    DiscountAmount = model.DiscountAmount,
                    CashPaid = model.CashPaid,
                    RemainingBalance = model.RemainingBalance,
                    MoneyDrawer = model.MoneyDrawer,
                    TotalAmount = model.TotalAmount - (model.TotalAmount*model.DiscountAmount/100),
                    UserId = userId,
                    Date = DateTime.Now,
                    BillItems = model.Items.Select(item => new BuyBillItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PurchasePrice = item.PurchasePrice,
                        SalePrice = item.SalePrice,
                        Total = item.Total,
                        InventoryId = item.InventoryId
                    }).ToList()
                };
                if(model.RemainingBalance > 0)
                {
                    var supplier = context.suppliers.FirstOrDefault(x => x.Id == model.SupplierId);
                    if(supplier != null)
                    {
                        supplier.Balance += model.RemainingBalance;
                    }
                }
                else if (model.RemainingBalance < 0)
                {
                    var supplier = context.suppliers.FirstOrDefault(x => x.Id == model.SupplierId);
                    if (supplier != null)
                    {
                        supplier.Balance -= Math.Abs(model.RemainingBalance);
                    }
                }
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                if (moneyDrawer != null)
                {
                    if (moneyDrawer.CurrentBalance < model.CashPaid && model.CashPaid != 0) return BadRequest("!رصيد الخزنة غير كافي");
                    moneyDrawer.CurrentBalance -= model.CashPaid;
                    context.moneyDrawer.Update(moneyDrawer);
                }

                foreach (var item in model.Items)
                {
                    var isFound = context.productSuppliers.Any(x => x.SupplierId == model.SupplierId && x.ProductId == item.ProductId);
                    var product = context.products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (product != null && !isFound)
                    {
                        product.Quantity += item.Quantity;
                        if(item.SalePrice > product.Price ) product.Price = item.SalePrice;
                        var productSupplier = new ProductSupplier
                        {
                            ProductId = item.ProductId,
                            SupplierId = model.SupplierId
                        };
                        context.productSuppliers.Add(productSupplier);

                        context.products.Update(product);
                    }
                    else
                    {
                        product.Quantity += item.Quantity;
                        if (item.SalePrice > product.Price) product.Price = item.SalePrice;
                        context.products.Update(product);
                    }

                    var inventoryProduct = context.inventoryProducts
                        .FirstOrDefault(ip => ip.ProductId == item.ProductId && ip.InventoryId == item.InventoryId);

                    if (inventoryProduct != null)
                    {
                        inventoryProduct.Quantity += item.Quantity;
                        context.inventoryProducts.Update(inventoryProduct);
                    }
                    else
                    {
                        var newInventoryProduct = new InventoryProduct
                        {
                            ProductId = item.ProductId,
                            InventoryId = item.InventoryId,
                            Quantity = item.Quantity
                        };
                        context.inventoryProducts.Add(newInventoryProduct);
                    }

                    var isBatch = context.productBatches.
                        FirstOrDefault(x => x.PurchasePrice == item.PurchasePrice && x.InventoryId == item.InventoryId && x.ProductId == item.ProductId && isFound == true);
                  if(isBatch == null)
                    {
                        var productBatch = new ProductBatch
                        {
                            ProductId = item.ProductId,
                            InventoryId = item.InventoryId,
                            Quantity = item.Quantity,
                            PurchasePrice = item.PurchasePrice,
                            PurchaseDate = DateTime.Now
                        };
                        context.productBatches.Add(productBatch);
                    }
                  else
                    {
                        isBatch.Quantity += item.Quantity;
                        isBatch.PurchaseDate = DateTime.Now;
                    }
                    
                }

                context.buyBill.Add(bill);

                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            model.Suppliers = context.suppliers.ToList();
            model.Products = context.products.ToList();
            model.Inventories = context.inventories.ToList();
            model.MoneyDrawers = context.moneyDrawer.ToList();
            model.Items = new List<BillItemViewModel>();

            return View(model);
        }

        

        [Permission("إنشاء فاتورة مبيعات")]
        [HttpGet]
        public IActionResult CreateSalesBill()
        {
            var model = new SalesBillViewModel
            {
                clients = context.clients.ToList(),
                MoneyDrawers = context.moneyDrawer.ToList(),
                inventories = context.inventories.ToList(),
                products = context.products.Where(p => p.Quantity > 0).ToList(),
                Employees = context.employees.ToList(),
                Items = new List<SalesBillItemViewModel>()
            };

            return View(model);
        }


        [Permission("إنشاء فاتورة مبيعات")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSalesBill(SalesBillViewModel model)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                

                var salesBill = new SalesBill
                {
                    
                    ClientId = model.ClientId,
                    TotalAmount = model.TotalAmount,
                    DiscountAmount = model.DiscountAmount,
                    CashPaid = model.CashPaid,
                    RemainingBalance = model.RemainingBalance,
                    MoneyDrawer = model.MoneyDrawer,
                    Date = DateTime.Now,
                    UserId = userId,
                    EmployeeId = model.EmployeeId,  
                    PaymentMethod = model.PaymentMethod,  
                    Items = new List<SalesBillItem>()
                };

                if (model.RemainingBalance > 0)
                {
                    var client = context.clients.FirstOrDefault(x => x.Id == model.ClientId);
                    if (client != null)
                    {
                        client.Balance = - model.RemainingBalance;
                    }
                }
                else if(model.RemainingBalance < 0)
                {
                    var client = context.clients.FirstOrDefault(x => x.Id == model.ClientId);
                    if (client != null)
                    {
                        client.Balance = Math.Abs( model.RemainingBalance);
                    }
                }

                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                if (moneyDrawer != null)
                {
                    moneyDrawer.CurrentBalance += model.CashPaid;
                    context.moneyDrawer.Update(moneyDrawer);
                }

                foreach (var item in model.Items)
                {
                    var product = context.products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product == null || product.Quantity < item.Quantity)
                    {
                        return BadRequest("الكمية الموجودة غير كافية");
                    }

                    int remainingQuantity = item.Quantity;
                    decimal itemTotalPrice = 0;

                    var productBatches = context.productBatches
                        .Where(pb => pb.InventoryId == item.InventoryId && pb.ProductId == item.ProductId && pb.Quantity > 0)
                        .OrderBy(pb => pb.PurchaseDate)
                        .ToList();

                    foreach (var batch in productBatches)
                    {
                        if (remainingQuantity <= 0) break;

                        int batchQuantityToDeduct = Math.Min(batch.Quantity, remainingQuantity);
                        decimal batchCost = batchQuantityToDeduct * batch.PurchasePrice;

                        batch.Quantity -= batchQuantityToDeduct;
                        remainingQuantity -= batchQuantityToDeduct;
                        itemTotalPrice += batchQuantityToDeduct * item.SalePrice;

                        var inventoryProduct = context.inventoryProducts
                            .FirstOrDefault(ip => ip.InventoryId == item.InventoryId && ip.ProductId == item.ProductId);

                        if (inventoryProduct != null)
                        {
                            inventoryProduct.Quantity -= batchQuantityToDeduct;
                            context.inventoryProducts.Update(inventoryProduct);
                        }

                        salesBill.Items.Add(new SalesBillItem
                        {
                            InventoryId = item.InventoryId,
                            ProductId = item.ProductId,
                            Quantity = batchQuantityToDeduct,
                            SalePrice = item.SalePrice,
                            TotalPrice = batchQuantityToDeduct * item.SalePrice
                        });
                    }

                    if (remainingQuantity > 0)
                    {
                        return BadRequest("Insufficient batch quantity to fulfill the order.");
                    }

                    product.Quantity -= item.Quantity;
                    context.products.Update(product);
                }

                context.salesBill.Add(salesBill);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            
            model.clients = context.clients.ToList();
            model.MoneyDrawers = context.moneyDrawer.ToList();
            model.inventories = context.inventories.ToList();
            model.products = context.products.ToList();
            model.Employees = context.employees.ToList(); // Add employees list for dropdown

            return View(model);
        }




        
        [HttpGet]
        public IActionResult GetAvailableStock(int productId, int inventoryId)
        {
            var availableQuantity = context.productBatches
                .Where(pb => pb.ProductId == productId && pb.InventoryId == inventoryId)
                .Sum(pb => pb.Quantity);

            return Json(new { availableQuantity = availableQuantity > 0 ? availableQuantity : 0 });
        }
        [HttpGet]
        public IActionResult GetProductPurchasePrice(int productId)
        {
            var latestBatch = context.productBatches
                                      .Where(pb => pb.ProductId == productId && pb.Quantity>0)
                                      .OrderBy(pb => pb.PurchaseDate)
                                      .FirstOrDefault();

            if (latestBatch != null)
            {
                return Json(new { purchasePrice = latestBatch.PurchasePrice });
            }
            return Json(new { purchasePrice = 0 });
        }


        [Permission("إنشاء فاتورة مردود شراء")]
        [HttpGet]
        public IActionResult CreatePurchaseReturnBill()
        {
            var model = new PurchaseBillViewModel
            {
                Suppliers = context.suppliers.ToList(),
                Inventories = context.inventories.ToList(),
                Products = context.products.ToList(),
                MoneyDrawers = context.moneyDrawer.ToList(),
                Items = new List<BillItemViewModel>()
            };

            return View(model);
        }

        [Permission("إنشاء فاتورة مردود شراء")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CreatePurchaseReturnBill(PurchaseBillViewModel model)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                if (model.DiscountMethod == "Percentage") model.DiscountAmount = (model.DiscountAmount * model.TotalAmount) / 100;

                model.TotalAmount = model.Items.Sum(item => item.PurchasePrice * item.Quantity);
                model.RemainingBalance = model.TotalAmount - model.DiscountAmount - model.CashPaid;

                var bill = new PurchaseReturnBill
                {
                    SupplierId = model.SupplierId,
                    PaymentMethod = model.PaymentMethod,
                    DiscountAmount = model.DiscountAmount,
                    CashPaid = model.CashPaid,
                    RemainingBalance = model.RemainingBalance,
                    MoneyDrawer = model.MoneyDrawer,
                    TotalAmount = model.TotalAmount,
                    UserId = userId,
                    Date = DateTime.Now,
                    BillItems = model.Items.Select(item => new PurchaseReturnBillItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PurchasePrice = item.PurchasePrice,
                        SalePrice = item.SalePrice,
                        Total = item.Total,
                        InventoryId = item.InventoryId
                    }).ToList()
                };
                if (model.RemainingBalance > 0)
                {
                    var supplier = context.suppliers.FirstOrDefault(x => x.Id == model.SupplierId);
                    if (supplier != null)
                    {
                        supplier.Balance -= model.RemainingBalance;
                    }
                }
                else if (model.RemainingBalance < 0)
                {
                    var supplier = context.suppliers.FirstOrDefault(x => x.Id == model.SupplierId);
                    if (supplier != null)
                    {
                        supplier.Balance += Math.Abs(model.RemainingBalance);
                    }
                }
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                if (moneyDrawer != null)
                {
                    moneyDrawer.CurrentBalance += model.CashPaid;
                    context.moneyDrawer.Update(moneyDrawer);
                }

                foreach (var item in model.Items)
                {
                    var quantityToReturn = item.Quantity;

                    var prduct = context.products.FirstOrDefault(x => x.Id == item.ProductId);
                    if(prduct != null) { prduct.Quantity -= quantityToReturn; }
                    var batch = context.productBatches
                        .FirstOrDefault(pb => pb.ProductId == item.ProductId &&
                                              pb.InventoryId == item.InventoryId &&
                                              pb.PurchasePrice == item.PurchasePrice);

                    if (batch != null)
                    {
                        batch.Quantity -= quantityToReturn;
                        context.productBatches.Update(batch);
                    }

                    var inventoryProduct = context.inventoryProducts
                        .FirstOrDefault(ip => ip.ProductId == item.ProductId && ip.InventoryId == item.InventoryId);

                    if (inventoryProduct != null)
                    {
                        inventoryProduct.Quantity -= item.Quantity;
                        context.inventoryProducts.Update(inventoryProduct);
                    }
                }

                context.purchaseReturnBills.Add(bill);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            model.Suppliers = context.suppliers.ToList();
            model.Products = context.products.ToList();
            model.Inventories = context.inventories.ToList();
            model.MoneyDrawers = context.moneyDrawer.ToList();
            model.Items = new List<BillItemViewModel>();
            return View(model);
        }

        [Permission("إنشاء فاتورة مردود مبيعات")]
        [HttpGet]
        public IActionResult CreateReturnSalesBill()
        {
            var model = new SalesBillViewModel
            {
                clients = context.clients.ToList(),
                MoneyDrawers = context.moneyDrawer.ToList(),
                inventories = context.inventories.ToList(),
                products = context.products.ToList(),
                Items = new List<SalesBillItemViewModel>()
            };

            return View(model);
        }

        [Permission("إنشاء فاتورة مردود مبيعات")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateReturnSalesBill(SalesBillViewModel model)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                var salesBill = new SalesReturnBill
                {
                    ClientId = model.ClientId,
                    TotalAmount = model.TotalAmount,
                    DiscountAmount = model.DiscountAmount,
                    CashPaid = model.CashPaid,
                    RemainingBalance = model.RemainingBalance,
                    PaymentMethod = model.PaymentMethod,
                    MoneyDrawer = model.MoneyDrawer,
                    UserId = userId,
                    Date = DateTime.Now,
                    Items = model.Items.Select(i => new SalesReturnBillItem
                    {
                        InventoryId = i.InventoryId,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        SalePrice = i.SalePrice,
                        TotalPrice = i.TotalPrice
                    }).ToList()
                };
                if (model.RemainingBalance > 0)
                {
                    var client = context.clients.FirstOrDefault(x => x.Id == model.ClientId);
                    if (client != null)
                    {
                        client.Balance += model.RemainingBalance;
                    }
                }
                else if (model.RemainingBalance > 0)
                {
                    var client = context.clients.FirstOrDefault(x => x.Id == model.ClientId);
                    if (client != null)
                    {
                        client.Balance -= Math.Abs(model.RemainingBalance);
                    }
                }
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                if (moneyDrawer != null)
                {
                    moneyDrawer.CurrentBalance -= model.CashPaid;
                    context.moneyDrawer.Update(moneyDrawer);
                }

                foreach (var item in model.Items)
                {
                    var quantityToReturn = item.Quantity;
                    var prduct = context.products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (prduct != null) { prduct.Quantity += quantityToReturn; }

                    var batches = context.productBatches
                        .Where(pb => pb.ProductId == item.ProductId && pb.InventoryId == item.InventoryId)
                        .OrderBy(pb => pb.PurchaseDate)
                        .ToList();

                    foreach (var batch in batches)
                    {
                        if (quantityToReturn <= 0) break;

                        var addBackQuantity = Math.Min(quantityToReturn, batch.Quantity);
                        batch.Quantity += addBackQuantity;
                        quantityToReturn -= addBackQuantity;

                        context.productBatches.Update(batch);
                    }

                    var inventoryProduct = context.inventoryProducts
                        .FirstOrDefault(ip => ip.ProductId == item.ProductId && ip.InventoryId == item.InventoryId);

                    if (inventoryProduct != null)
                    {
                        inventoryProduct.Quantity += item.Quantity;
                        context.inventoryProducts.Update(inventoryProduct);
                    }
                }

                context.salesReturnBills.Add(salesBill);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            model.clients = context.clients.ToList();
            model.MoneyDrawers = context.moneyDrawer.ToList();
            model.inventories = context.inventories.ToList();
            model.products = context.products.ToList();
            return View(model);
        }

        [Permission("إضافة مصروف")]
        [HttpGet]
        public IActionResult AddExpense()
        {
            var expense = new ExpenseViewModel();
            expense.Expenses = context.expense.Where(x => x.Amount > 0).ToList();
            expense.moneyDrawers=context.moneyDrawer.ToList();
            return View(expense);
        }

        [Permission("إضافة مصروف")]
        [HttpPost]
        public IActionResult AddNewExpense(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var expense = new Expense();
                expense.Item = model.Item;
                expense.Amount = 0;
                expense.MoneyDrawerId = 1;
                expense.UserId = userId;
                context.expense.Add(expense);
                context.SaveChanges();
                return RedirectToAction("AddExpense");
            }
            return View(model);
        }
        [Permission("إضافة مصروف")]
        public IActionResult AddExpense(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var Expense in model.Expenses)
                {
                    var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    var isFound = context.expense.FirstOrDefault(x => x.Item == Expense.Item);
                    if (isFound != null && isFound.Amount == 0)
                    {
                        isFound.Amount = Expense.Amount;
                        isFound.MoneyDrawerId = Expense.MoneyDrawerId;
                        isFound.ExpenseDate = DateTime.Now;
                    }
                    else
                    {
                        var expense = new Expense
                        {
                            Item = Expense.Item,
                            Amount = Expense.Amount,
                            MoneyDrawerId = Expense.MoneyDrawerId,
                            ExpenseDate = DateTime.Now,
                            UserId = userId
                        };
                        context.expense.Add(expense);
                    }
                    var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Id == Expense.MoneyDrawerId);
                    moneyDrawer.CurrentBalance -= Expense.Amount;
                    context.moneyDrawer.Update(moneyDrawer);
                }

                context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            model.Expenses = context.expense.ToList();
            model.moneyDrawers = context.moneyDrawer.ToList();
            return View(model);
        }
        [Permission("التحويل بين الخزنات")]
        [HttpGet]
        public IActionResult TransferBetweenMoneyDrawers()
        {
            var model = new TransferMoneyViewModel
            {
                moneyDrawers = context.moneyDrawer.ToList()
            };
            return View(model);
        }
        [Permission("إضافة خزنة جديدة")]
        [HttpPost]
        public IActionResult AddNewMoneyDrawer(TransferMoneyViewModel model)
        {
            if(ModelState.IsValid)
            {
                var MD = new MoneyDrawer
                {
                    Name = model.NewMoneyDarwer,
                    CurrentBalance = 0
                };
                context.moneyDrawer.Add(MD);
                context.SaveChanges();
                return RedirectToAction("TransferBetweenMoneyDrawers", "Admin");
            }
            return View("TransferBetweenMoneyDrawers",model);
        }
        [Permission("التحويل بين الخزنات")]
        [HttpPost]
        public IActionResult SaveTransfer(TransferMoneyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var from = context.moneyDrawer.FirstOrDefault(x => x.Id == model.FromId);
                var to = context.moneyDrawer.FirstOrDefault(y => y.Id == model.ToId);

                if (from != null && to != null)
                {
                    
                    from.CurrentBalance -= model.TotalAmount;
                    to.CurrentBalance += model.TotalAmount;

                    
                    context.moneyDrawer.Update(from);
                    context.moneyDrawer.Update(to);

                    var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    var transfer = new TransferBetweenMoneyDrawers
                    {
                        FromMoneyDrawerId = model.FromId,
                        ToMoneyDrawerId = model.ToId,
                        Amount = model.TotalAmount,
                        UserId = userId,
                        Date = DateTime.Now 
                    };

                    
                    context.Transfers.Add(transfer);

                    
                    context.SaveChanges();
                }

                return RedirectToAction("TransferBetweenMoneyDrawers", "Admin");
            }

            return View("TransferBetweenMoneyDrawers", model);
        }

        [Permission("التحويل بين المخازن")]
        [HttpGet]
        public IActionResult TransferBetweenInventories()
        {
            var model = new TransferProductsViewModels
            {
                inventories = context.inventories.ToList(),
                products = context.products.Where(x => x.Quantity > 0).ToList()
            };

            return View(model);
        }
        [Permission("التحويل بين المخازن")]
        [HttpPost]
        public async Task<IActionResult> SaveTransferBetweenInventories(TransferViewModel model)
        {
            int fromInventoryId = model.FromInventoryId;
            int toInventoryId = model.ToInventoryId;
            var transfers = model.Transfers;

            if (fromInventoryId == toInventoryId)
                return BadRequest("Cannot transfer to the same inventory.");

            foreach (var transfer in transfers)
            {
                var fromInventoryProduct = context.inventoryProducts
                    .FirstOrDefault(ip => ip.InventoryId == fromInventoryId && ip.ProductId == transfer.ProductId);

                if (fromInventoryProduct == null || fromInventoryProduct.Quantity < transfer.Quantity)
                    return BadRequest("Insufficient quantity for transfer.");

                var toInventoryProduct = context.inventoryProducts
                    .FirstOrDefault(ip => ip.InventoryId == toInventoryId && ip.ProductId == transfer.ProductId);

                int quantityToTransfer = transfer.Quantity;
                var productBatches = context.productBatches
                    .Where(pb => pb.InventoryId == fromInventoryId && pb.ProductId == transfer.ProductId && pb.Quantity > 0)
                    .OrderBy(pb => pb.PurchaseDate)
                    .ToList();

                foreach (var batch in productBatches)
                {
                    if (quantityToTransfer <= 0) break;

                    int transferBatchQuantity = Math.Min(batch.Quantity, quantityToTransfer);
                    batch.Quantity -= transferBatchQuantity;
                    quantityToTransfer -= transferBatchQuantity;

                    var targetBatch = context.productBatches
                        .FirstOrDefault(pb => pb.InventoryId == toInventoryId && pb.ProductId == transfer.ProductId && pb.PurchasePrice == batch.PurchasePrice);

                    if (targetBatch == null)
                    {
                        targetBatch = new ProductBatch
                        {
                            ProductId = transfer.ProductId,
                            InventoryId = toInventoryId,
                            Quantity = transferBatchQuantity,
                            PurchasePrice = batch.PurchasePrice,
                            PurchaseDate = DateTime.Now
                        };
                        context.productBatches.Add(targetBatch);
                    }
                    else
                    {
                        targetBatch.Quantity += transferBatchQuantity;
                        context.productBatches.Update(targetBatch);
                    }
                }

                // Update inventory quantities
                fromInventoryProduct.Quantity -= transfer.Quantity;

                if (toInventoryProduct == null)
                {
                    context.inventoryProducts.Add(new InventoryProduct
                    {
                        InventoryId = toInventoryId,
                        ProductId = transfer.ProductId,
                        Quantity = transfer.Quantity
                    });
                }
                else
                {
                    toInventoryProduct.Quantity += transfer.Quantity;
                }
            }

            await context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult GetProducts(int fromInventoryId, string searchQuery)
        {
            var products = context.inventoryProducts
                .Where(ip => ip.InventoryId == fromInventoryId &&
                             (string.IsNullOrEmpty(searchQuery) || ip.Product.Name.Contains(searchQuery)))
                .Select(ip => new {
                    ip.Product.Id,
                    ip.Product.Name,
                    Inventory = ip.Inventory.Name,
                    Quantity = ip.Quantity
                }).ToList();

            return Json(products);
        }

        [Permission("إضافة مخزون جديد")]
        [HttpPost]
        public IActionResult AddNewInventory(TransferProductsViewModels model)
        {
            if (ModelState.IsValid)
            {
                var inv = new Inventory
                {
                    Name = model.Name
                };
               context.inventories.Add(inv);
                context.SaveChanges();
                return RedirectToAction("TransferBetweenInventories");
            }
            model.inventories = context.inventories.ToList();
            model.products = context.products.Where(x => x.Quantity > 0).ToList();
            return View("TransferBetweenInventories",model);
        }
        [Permission("إضافة إيصال من العميل")]
        [HttpGet]
        public IActionResult AddFromClientReceipt()
        {
            
           

            var viewModel = new ReceiptViewModel
            {
                
                moneyDrawers = context.moneyDrawer.ToList()
            };

            return View(viewModel);
        }

        
        [HttpGet]
        public IActionResult GetClientBalance(int clientId)
        {
            var client = context.clients.Find(clientId);
            if (client == null)
                return NotFound("Client not found.");

            return Ok(client.Balance);
        }
        [HttpGet]
        public IActionResult SearchClients(string term)
        {
            var clients = context.clients
                .Where(c => c.Name.Contains(term))
                .Select(c => new { c.Id, c.Name })
                .ToList();

            return Ok(clients);
        }

        [Permission("إضافة إيصال من العميل")]
        [HttpPost]
        public IActionResult SaveFromClientReceipt(ReceiptViewModel model)
        {
            if(ModelState.IsValid)
            {
                var client = context.clients.FirstOrDefault(x => x.Id == model.Id);
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Id == model.MoneyDrawerId);
                if (client == null)
                {
                    ModelState.AddModelError(string.Empty, "Client not found.");
                    return View(model);
                }

                client.Balance += model.PaymentAmount;
                moneyDrawer.CurrentBalance += model.PaymentAmount;

                var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

                var receipt = new ClientReceipt
                {
                    ClientId = model.Id,
                    AmountPaid = model.PaymentAmount,
                    Type = "استلام",
                    MoneyDrawer = moneyDrawer.Name,
                    UserId = userId,
                    Date = DateTime.Now
                };

                context.clientReceipts.Add(receipt);
                context.SaveChanges();

               
                return RedirectToAction("Index", "Home");
            }
            model.moneyDrawers = context.moneyDrawer.ToList();
            return View("AddFromClientReceipt", model);
        }

        [Permission("إضافة إيصال للعميل")]
        [HttpGet]
        public IActionResult AddToClientReceipt()
        {



            var viewModel = new ReceiptViewModel
            {
                
                moneyDrawers = context.moneyDrawer.ToList()
            };

            return View(viewModel);
        }
        [Permission("إضافة إيصال للعميل")]
        [HttpPost]
        public IActionResult SaveToClientReceipt(ReceiptViewModel model)
        {
            if(ModelState.IsValid)
            {
                var client = context.clients.FirstOrDefault(x => x.Id == model.Id);
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Id == model.MoneyDrawerId);
                if (client == null)
                {
                    ModelState.AddModelError(string.Empty, "Client not found.");
                    return View(model);
                }

                client.Balance -= model.PaymentAmount;
                moneyDrawer.CurrentBalance -= model.PaymentAmount;

                var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

                var receipt = new ClientReceipt
                {
                    ClientId = model.Id,
                    AmountPaid = model.PaymentAmount,
                    Type = "صرف",
                    MoneyDrawer = moneyDrawer.Name,
                    UserId = userId,
                    Date = DateTime.Now
                };

                context.clientReceipts.Add(receipt);
                context.SaveChanges();

                
                return RedirectToAction("Index", "Home");
            }
            model.moneyDrawers = context.moneyDrawer.ToList();
            return View("AddToClientReceipt", model);
        }

        [Permission("عرض العملاء")]
        public IActionResult ViewClients()
        {
            var clients = context.clients.ToList();
            return View(clients);
        }
        [Permission("كشف حساب العميل")]
        [HttpGet]
        public IActionResult AccountStatement(int clientId)
        {
            // Fetch the client and their current balance
            var client = context.clients.FirstOrDefault(c => c.Id == clientId);
            if (client == null)
            {
                return NotFound(); // Handle if client doesn't exist
            }

            var viewModel = new AccountStatementViewModel
            {
                Id = clientId,
                Name = client.Name,
                Balance = client.Balance,  // Add client balance to the view model

                Receipts = context.clientReceipts
                    .Where(r => r.ClientId == clientId)
                    .Select(r => new ReceiptVM
                    {
                        Id = r.Id,
                        Date = r.Date,
                        AmountPaid = r.AmountPaid,
                        Type = r.Type ?? "N/A"
                    }).ToList(),

                SalesBills = context.salesBill
                    .Where(sb => sb.ClientId == clientId)
                    .SelectMany(sb => sb.Items, (sb, item) => new SalesBillVM
                    {
                        Id = sb.Id,
                        Date = sb.Date,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        SalePrice = item.SalePrice,
                        TotalPrice = item.TotalPrice,
                        PaymentMethod = sb.PaymentMethod, 
                        SalesPoint = sb.MoneyDrawer,
                        SalesPerson = sb.Employee.Name ?? "غير معروف"
                    }).ToList()
            };

            
            viewModel.TotalAmount = viewModel.SalesBills.Sum(b => b.TotalPrice);
            viewModel.TotalPaid = viewModel.Receipts.Sum(r => r.AmountPaid)+context.salesBill.Where(x=>x.ClientId==clientId).Sum(x=>x.CashPaid);
            viewModel.TotalRemaining = viewModel.TotalAmount - viewModel.TotalPaid;

            return View("AccountStatement", viewModel); // Ensure that the view is named AccountStatement.cshtml
        }

        [Permission("كشف حساب العميل")]
        [HttpGet]
        public IActionResult FilteredAccountStatement(int clientId, DateTime startDate, DateTime endDate)
        {
            // Fetch the client and their current balance
            var client = context.clients.FirstOrDefault(c => c.Id == clientId);
            if (client == null)
            {
                return NotFound(); // Handle if client doesn't exist
            }

            var viewModel = new AccountStatementViewModel
            {
                Id = clientId,
                Name = client.Name,
                Balance = client.Balance,  // Add client balance to the view model

                Receipts = context.clientReceipts
                    .Where(r => r.ClientId == clientId && r.Date >= startDate && r.Date <= endDate)
                    .Select(r => new ReceiptVM
                    {
                        Id = r.Id,
                        Date = r.Date,
                        AmountPaid = r.AmountPaid,
                        Type = r.Type ?? "N/A"
                    }).ToList(),

                SalesBills = context.salesBill
                    .Where(sb => sb.ClientId == clientId && sb.Date >= startDate && sb.Date <= endDate)
                    .SelectMany(sb => sb.Items, (sb, item) => new SalesBillVM
                    {
                        Id = sb.Id,
                        Date = sb.Date,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        SalePrice = item.SalePrice,
                        TotalPrice = item.TotalPrice,
                        PaymentMethod = sb.PaymentMethod, // Assuming cash, adapt as needed
                        SalesPoint = sb.MoneyDrawer,
                        SalesPerson = sb.Employee.Name?? "غير معروف"
                    }).ToList()
            };

            // Calculate the totals
            viewModel.TotalAmount = viewModel.SalesBills.Sum(b => b.TotalPrice);
            viewModel.TotalPaid = viewModel.Receipts.Sum(r => r.AmountPaid)+context.salesBill.Where(x => x.ClientId == clientId).Sum(x => x.CashPaid);
            viewModel.TotalRemaining = viewModel.TotalAmount - viewModel.TotalPaid;

            return View("AccountStatement", viewModel); // Reuse the same view for filtered results
        }
        [Permission("حذف عميل")]
        [HttpGet]
        public IActionResult DeleteClient(int clientId)
        {
           var client =  context.clients.FirstOrDefault(x => x.Id == clientId);
            if (client != null)
            {
                context.clients.Remove(client);
                context.SaveChanges();
                return RedirectToAction("ViewClients", "Admin");
            }
            return NotFound();
        }
        [Permission("إضافة إيصال من المورد")]
        [HttpGet]
        public IActionResult AddFromSupplierReceipt()
        {



            var viewModel = new ReceiptViewModel
            {

                moneyDrawers = context.moneyDrawer.ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetSupplierBalance(int supplierId)
        {
            var supplier = context.suppliers.Find(supplierId);
            if (supplier == null)
                return NotFound("!هذا المورد غير موجود");

            return Ok(supplier.Balance);
        }
        [HttpGet]
        public IActionResult SearchSuppliers(string term)
        {
            var suppliers = context.suppliers
                .Where(c => c.Name.Contains(term))
                .Select(c => new { c.Id, c.Name })
                .ToList();

            return Ok(suppliers);
        }
        [Permission("إضافة إيصال من المورد")]
        [HttpPost]
        public IActionResult SaveFromSupplierReceipt(ReceiptViewModel model)
        {
            if(ModelState.IsValid)
            {
                var supplier = context.suppliers.FirstOrDefault(x => x.Id == model.Id);
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Id == model.MoneyDrawerId);
                if (supplier == null)
                {
                    ModelState.AddModelError(string.Empty, "! هذا المورد غير موجود ");
                    return View(model);
                }

                supplier.Balance += model.PaymentAmount;
                moneyDrawer.CurrentBalance += model.PaymentAmount;

                var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

                var receipt = new SupplierReceipt
                {
                    SupplierId = model.Id,
                    AmountPaid = model.PaymentAmount,
                    Type = "استلام",
                    MoneyDrawer = moneyDrawer.Name,
                    UserId = userId,
                    Date = DateTime.Now
                };

                context.supplierReceipts.Add(receipt);
                context.SaveChanges();

                
                return RedirectToAction("Index", "Home");
            }
            model.moneyDrawers = context.moneyDrawer.ToList();
            return View("AddFromSupplierReceipt",model);
        }
        [Permission("إضافة إيصال للمورد")]
        [HttpGet]
        public IActionResult AddToSupplierReceipt()
        {



            var viewModel = new ReceiptViewModel
            {

                moneyDrawers = context.moneyDrawer.ToList()
            };

            return View(viewModel);
        }
        [Permission("إضافة إيصال للمورد")]
        [HttpPost]
        public IActionResult SaveToSupplierReceipt(ReceiptViewModel model)
        {
            if(ModelState.IsValid)
            {
                var supplier = context.suppliers.FirstOrDefault(x => x.Id == model.Id);
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Id == model.MoneyDrawerId);
                if (supplier == null)
                {
                    ModelState.AddModelError(string.Empty, "!هذا المورد غير موجود ");
                    return View(model);
                }

                supplier.Balance -= model.PaymentAmount;
                moneyDrawer.CurrentBalance -= model.PaymentAmount;


                var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

                var receipt = new SupplierReceipt
                {
                    SupplierId = model.Id,
                    AmountPaid = model.PaymentAmount,
                    Type = "صرف",
                    MoneyDrawer = moneyDrawer.Name,
                    UserId = userId,
                    Date = DateTime.Now
                };

                context.supplierReceipts.Add(receipt);
                context.SaveChanges();

                
                return RedirectToAction("Index", "Home");
            }
            model.moneyDrawers = context.moneyDrawer.ToList();
            return View("AddToSupplierReceipt", model);
        }
        [Permission("عرض الموردين")]
        [HttpGet]
        public IActionResult ViewSuppliers()
        {
            var suppliers = context.suppliers.ToList();
            return View(suppliers);
        }
        [Permission("حذف مورد")]
        [HttpGet]
        public IActionResult DeleteSupplier(int supplierId)
        {
            var supplier = context.suppliers.FirstOrDefault(x => x.Id == supplierId);
            if (supplier != null)
            {
                context.suppliers.Remove(supplier);
                context.SaveChanges();
                return RedirectToAction("ViewSuppliers", "Admin");
            }
            return NotFound("!هذا المورد غير موجود");
        }
        [Permission("كشف حساب المورد")]
        [HttpGet]
        public IActionResult SupplierAccountStatement(int supplierId)
        {
            // Fetch the client and their current balance
            var client = context.suppliers.FirstOrDefault(c => c.Id == supplierId);
            if (client == null)
            {
                return NotFound(); // Handle if client doesn't exist
            }

            var viewModel = new AccountStatementViewModel
            {
                Id = supplierId,
                Name = client.Name,
                Balance = client.Balance,  // Add client balance to the view model

                Receipts = context.supplierReceipts
                    .Where(r => r.SupplierId == supplierId)
                    .Select(r => new ReceiptVM
                    {
                        Id = r.Id,
                        Date = r.Date,
                        AmountPaid = r.AmountPaid,
                        Type = r.Type ?? "N/A"
                    }).ToList(),

                PurchaseBills = context.buyBill
                    .Where(sb => sb.SupplierId == supplierId)
                    .SelectMany(sb => sb.BillItems, (sb, item) => new PurchaseBillVM
                    {
                        Id = sb.Id,
                        Date = sb.Date,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        PurchasePrice = item.PurchasePrice,
                        CashPaid = sb.CashPaid,
                        TotalPrice = item.Total,
                        PaymentMethod = sb.PaymentMethod, // Assuming cash, adapt as needed
                        SalesPoint = sb.MoneyDrawer,
                        
                    }).ToList(),
                TotalCashpaid = context.buyBill.Where(x => x.SupplierId == supplierId).Sum(x => x.CashPaid)
                    
            };

            // Calculate the totals
            viewModel.TotalAmount = viewModel.PurchaseBills.Sum(b => b.TotalPrice);
            viewModel.TotalPaid = viewModel.Receipts.Where(x=>x.Type=="صرف").Sum(r => r.AmountPaid)+viewModel.TotalCashpaid;
            viewModel.TotalRemaining = viewModel.TotalAmount - viewModel.TotalPaid;

            return View("SupplierAccountStatement", viewModel); // Ensure that the view is named AccountStatement.cshtml
        }

        [Permission("كشف حساب المورد")]
        [HttpGet]
        public IActionResult SupplierFilteredAccountStatement(int supplierId, DateTime startDate, DateTime endDate)
        {
            // Fetch the client and their current balance
            var client = context.clients.FirstOrDefault(c => c.Id == supplierId);
            if (client == null)
            {
                return NotFound(); // Handle if client doesn't exist
            }

            var viewModel = new AccountStatementViewModel
            {
                Id = supplierId,
                Name = client.Name,
                Balance = client.Balance,  // Add client balance to the view model

                Receipts = context.supplierReceipts
                    .Where(r => r.SupplierId == supplierId && r.Date >= startDate && r.Date <= endDate)
                    .Select(r => new ReceiptVM
                    {
                        Id = r.Id,
                        Date = r.Date,
                        AmountPaid = r.AmountPaid,
                        Type = r.Type ?? "N/A"
                    }).ToList(),

                PurchaseBills = context.buyBill
                    .Where(sb => sb.SupplierId == supplierId && sb.Date >= startDate && sb.Date <= endDate)
                    .SelectMany(sb => sb.BillItems, (sb, item) => new PurchaseBillVM
                    {
                        Id = sb.Id,
                        Date = sb.Date,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        PurchasePrice = item.PurchasePrice,
                        TotalPrice = item.Total,
                        PaymentMethod = sb.PaymentMethod, 
                        SalesPoint = sb.MoneyDrawer,
                        
                    }).ToList()
            };

            // Calculate the totals
            viewModel.TotalAmount = viewModel.PurchaseBills.Sum(b => b.TotalPrice);
            viewModel.TotalPaid = viewModel.Receipts.Sum(r => r.AmountPaid);
            viewModel.TotalRemaining = viewModel.TotalAmount - viewModel.TotalPaid;

            return View("SupplierAccountStatement", viewModel); // Reuse the same view for filtered results
        }

        [Permission("قائمة الأسعار")]
        [HttpGet]
        public IActionResult PriceList()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ClientsSearch(string query)
        {
            var clients = await context.clients
                .Where(c => c.Name.Contains(query))
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();
            return Ok(clients);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string query)
        {
            var products = await context.products
                .Where(p => p.Name.Contains(query))
                .Select(p => new { p.Id, p.Name, p.Price })
                .ToListAsync();
            return Ok(products);
        }
        [Permission("عرض نقص الأصناف")]
        [HttpGet]
        public IActionResult ViewProductsShortcomings()
        {
            var products = context.products.Where(x => x.Quantity <= 0).ToList();
            return View(products);
        }
        [Permission("أقساط متأخرة")]
        [HttpGet]
        public async Task<IActionResult> LateInstallments()
        {
            DateTime currentDate = DateTime.Now;

            
            var lateInstallments =  context.salesBill
                .Include(bill => bill.Client) 
                .Where(bill => bill.RemainingBalance > 0 && bill.Client != null && bill.Client.Balance < 0)
                .AsEnumerable()
                .GroupBy(bill => bill.ClientId)
                .Select(group => group
                    .OrderByDescending(bill => bill.Date)
                    .First())
                .Select(bill => new LateInstallmentsViewModel
                {
                    Id = bill.Client.Id,
                    Name = bill.Client.Name,
                    PhoneNumber = bill.Client.PhoneNumber,
                    Balance = bill.Client.Balance,
                    DaysLate = (int)(currentDate - bill.Date).TotalDays
                })
                .ToList();

            return View(lateInstallments);
        }
        [Permission("الموقف المالي")]
        [HttpGet]
        public IActionResult FinancialPosition()
        {
            var forClients = context.clients.Where(x => x.Balance > 0).Sum(x => x.Balance);
            var onClients = context.clients.Where(x => x.Balance < 0).Sum(x => x.Balance);
            var onSuppliers = context.suppliers.Where(x => x.Balance < 0).Sum(x => x.Balance);
            var forSuppliers = context.suppliers.Where(x => x.Balance > 0).Sum(x => x.Balance);
            var drawersBalance = context.moneyDrawer.Sum(x => x.CurrentBalance);
            var inventoriesBalance = context.productBatches
                .Sum(pb => pb.Quantity * pb.PurchasePrice);

            var model = new FinancialPositionViewModel
            {
                ForClients = forClients,
                OnClients = Math.Abs(onClients),
                OnSuppliers = Math.Abs(onSuppliers),
                ForSuppliers = forSuppliers,
                DrawersActualBalance = drawersBalance,
                InventoriesActualBalance = inventoriesBalance,
                ActualTotalBalance = drawersBalance + inventoriesBalance,
                TotalForUs = Math.Abs(onClients) + Math.Abs(onSuppliers),
                TotalOnUs = forClients + forSuppliers,
            };

            model.FinalPosition = (model.ActualTotalBalance + model.TotalForUs) - model.TotalOnUs;
            return View(model);
        }
        [Permission("حركة الأصناف")]
        [HttpGet]
        public IActionResult ItemMovement()
        {
            var drawres = context.moneyDrawer.ToList();
            return View(drawres);
        }
        [HttpGet]
        public async Task<IActionResult> GetFilteredReport(string productName, string moneyDrawer, DateTime? startDate, DateTime? endDate, string operationType)
        {
            IQueryable<dynamic> query;

            // Based on the selected operation type, query the relevant table
            switch (operationType)
            {
                case "BuyBill": // BuyBill
                    query = context.buyBill
                        .Include(bb => bb.Supplier)
                        .Where(bb => (string.IsNullOrEmpty(productName) || bb.BillItems.Any(bi => bi.Product.Name.Contains(productName))) &&
                                     (string.IsNullOrEmpty(moneyDrawer) || bb.MoneyDrawer == moneyDrawer) &&
                                     (!startDate.HasValue || bb.Date >= startDate.Value) &&
                                     (!endDate.HasValue || bb.Date <= endDate.Value))
                        .SelectMany(bb => bb.BillItems
                            .Where(bi => bi.Product.Name.Contains(productName)), (bb, bi) => new
                            {
                                bb.Date,
                                bb.MoneyDrawer,
                                ProductName = bi.Product.Name,
                                Price = bi.Product.Price,
                                Quantity = bi.Quantity,
                                Name = bb.Supplier.Name,
                                RemainingQuantity = context.products.FirstOrDefault(x => x.Name == productName).Quantity
                            });
                    break;

                case "SalesBill": // SalesBill
                    query = context.salesBill
                        .Include(sb => sb.Client)
                        .Where(sb => (string.IsNullOrEmpty(productName) || sb.Items.Any(i => i.Product.Name.Contains(productName))) &&
                                     (string.IsNullOrEmpty(moneyDrawer) || sb.MoneyDrawer == moneyDrawer) &&
                                     (!startDate.HasValue || sb.Date >= startDate.Value) &&
                                     (!endDate.HasValue || sb.Date <= endDate.Value))
                        .SelectMany(sb => sb.Items
                            .Where(item => item.Product.Name.Contains(productName)), (sb, item) => new
                            {
                                sb.Date,
                                sb.MoneyDrawer,
                                ProductName = item.Product.Name,
                                Price = item.Product.Price,
                                Quantity = item.Quantity,
                                Name = sb.Client.Name,
                                RemainingQuantity = context.products.FirstOrDefault(x => x.Name == productName).Quantity
                            });
                    break;

                case "SalesReturnBill": // SalesReturnBill
                    query = context.salesReturnBills
                        .Include(srb => srb.client)
                        .Where(srb => (string.IsNullOrEmpty(productName) || srb.Items.Any(i => i.Product.Name.Contains(productName))) &&
                                     (string.IsNullOrEmpty(moneyDrawer) || srb.MoneyDrawer == moneyDrawer) &&
                                     (!startDate.HasValue || srb.Date >= startDate.Value) &&
                                     (!endDate.HasValue || srb.Date <= endDate.Value))
                        .SelectMany(srb => srb.Items
                            .Where(item => item.Product.Name.Contains(productName)), (srb, item) => new
                            {
                                srb.Date,
                                srb.MoneyDrawer,
                                ProductName = item.Product.Name,
                                Price = item.Product.Price,
                                Quantity = item.Quantity,
                                Name = srb.client.Name,
                                RemainingQuantity = context.products.FirstOrDefault(x => x.Name == productName).Quantity
                            });
                    break;

                case "PurchaseReturnBill":
                    query = context.purchaseReturnBills
                        .Include(pr => pr.Supplier)
                        .Where(pr => (string.IsNullOrEmpty(productName) || pr.BillItems.Any(bi => bi.Product.Name.Contains(productName))) &&
                                     (string.IsNullOrEmpty(moneyDrawer) || pr.MoneyDrawer == moneyDrawer) &&
                                     (!startDate.HasValue || pr.Date >= startDate.Value) &&
                                     (!endDate.HasValue || pr.Date <= endDate.Value))
                        .SelectMany(pr => pr.BillItems
                            .Where(bi => bi.Product.Name.Contains(productName)), (pr, bi) => new
                            {
                                pr.Date,
                                pr.MoneyDrawer,
                                ProductName = bi.Product.Name,
                                Price = bi.Product.Price,
                                Quantity = bi.Quantity,
                                Name = pr.Supplier.Name,
                                RemainingQuantity = context.products.FirstOrDefault(x => x.Name == productName).Quantity
                            });
                    break;

                default:
                    // If 'all' is selected or an invalid type, return an empty result or a generic query.
                    query = Enumerable.Empty<object>().AsQueryable();
                    break;
            }

            // Execute the query asynchronously and return the result as a JSON response
            var result = await query.ToListAsync();

            // Return the filtered data in JSON format
            return Json(result);
        }
        [Permission("عرض المصروفات اليومية")]
        public IActionResult ViewDailyExpenses()
        {
            var items = context.expense.Select(x => x.Item).Distinct();
            return View(items);
        }

        
        [HttpGet]
        public IActionResult GetFilteredExpenses(DateTime selectedDate, string category)
        {
            // Calculate the start and end of the day for the selected date
            DateTime startOfDay = selectedDate.Date;
            DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            // Filter expenses based on the date and category
            var expenses = context.expense
                .Where(e => e.ExpenseDate >= startOfDay && e.ExpenseDate <= endOfDay
                            && (category == "all" || e.Item == category))
                .Select(e => new
                {
                    e.ExpenseDate,
                    e.Item,
                    e.Amount,
                    MoneyDrawerName = e.MoneyDrawer.Name ?? "Unknown"
                })
                .ToList();

            return Json(expenses);
        }
        [Permission("عرض المبيعات اليومية")]
        [HttpGet]
        public IActionResult ViewDailySales()
        {
            var drawers = context.moneyDrawer.Select(x => x.Name).ToList();
            return View(drawers);
        }
        
        [HttpGet]
        public IActionResult GetDailySalesReport(DateTime selectedDate, string drawer)
        {
            // Calculate the start and end of the day for the selected date
            DateTime startOfDay = selectedDate.Date;
            DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            // Query to get sales bills on the specified date and drawer
            var salesData = context.salesBill
                .Where(s => s.Date >= startOfDay && s.Date <= endOfDay
                            && (drawer == "all" || s.MoneyDrawer == drawer))
                 .Select(x => new DailySalesViewModel
                 {
                    Id=x.Id,
                    Date = x.Date,
                    TotalAmount =x.TotalAmount,
                    CashPaid=x.CashPaid,
                    RemainingBalance=x.RemainingBalance,
                    ClientName = x.Client.Name?? "غير معروف",
                    UserName = x.User.UserName?? "غير معروف"
                 })
                .ToList();

            return Json(salesData);
        }
        [Permission("عرض عمليات الخزنة")]
        [HttpGet]
        public IActionResult ViewDrawerOperations()
        {
            var drawers = context.moneyDrawer.Select(x => x.Name).ToList();
            return View(drawers);
        }
        [HttpGet]
        public IActionResult GetDrawerBalance(string moneyDrawer)
        {
            if (string.IsNullOrEmpty(moneyDrawer))
                return BadRequest("Money drawer is not specified.");

            var drawer = context.moneyDrawer.FirstOrDefault(d => d.Name == moneyDrawer);
            if (drawer == null)
                return NotFound("Money drawer not found.");

            return Ok(drawer.CurrentBalance);
        }
        [HttpGet]
        public IActionResult GetSalesBillItems(int billId)
        {
            var billItems = context.SalesBillItem
                .Where(item => item.SalesBillId == billId)
                .Select(item => new
                {
                    
                    item.Product.Name,
                    item.Quantity,
                    item.SalePrice,
                    item.TotalPrice
                })
                .ToList();

            return Json(billItems);
        }

        [HttpGet]
        public IActionResult ViewSpecificDrawerOperations(string moneyDrawer)
        {
            
            return View("ViewSpecificDrawerOperations", moneyDrawer);
        }
        [HttpGet]
        public IActionResult Filtered(DateTime? startDate, DateTime? endDate, string moneyDrawer)
        {
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            var buyBills = context.buyBill
                .Where(b => b.Date >= startDate && b.Date <= endDate && b.MoneyDrawer == moneyDrawer)
                .Select(b => new
                {
                    Time = b.Date.ToString("hh:mm tt"),
                    Date = b.Date.ToString("yyyy-MM-dd"),
                    Revenue = 0,
                    Expense = b.CashPaid,
                    Description = $"فاتورة شراء {b.Id} المورد: {b.Supplier.Name}",
                    User = b.User.UserName ?? "غير معروف"
                });

            var salesBills = context.salesBill
                .Where(s => s.Date >= startDate && s.Date <= endDate && s.MoneyDrawer == moneyDrawer)
                .Select(s => new
                {
                    Time = s.Date.ToString("hh:mm tt"),
                    Date = s.Date.ToString("yyyy-MM-dd"),
                    Revenue = s.CashPaid,
                    Expense = 0,
                    Description = $"فاتورة مبيعات {s.Id} العميل: {s.Client.Name}",
                    User = s.User.UserName ?? "غير معروف"
                });

            var salesReturnBills = context.salesReturnBills
                .Where(s => s.Date >= startDate && s.Date <= endDate && s.MoneyDrawer == moneyDrawer)
                .Select(s => new
                {
                    Time = s.Date.ToString("hh:mm tt"),
                    Date = s.Date.ToString("yyyy-MM-dd"),
                    Revenue = 0,
                    Expense = s.CashPaid,
                    Description = $"مرتجع مبيعات {s.Id} العميل: {s.client.Name}",
                    User = s.User.UserName ?? "غير معروف"
                });

            var purchaseReturnBills = context.purchaseReturnBills
                .Where(p => p.Date >= startDate && p.Date <= endDate && p.MoneyDrawer == moneyDrawer)
                .Select(p => new
                {
                    Time = p.Date.ToString("hh:mm tt"),
                    Date = p.Date.ToString("yyyy-MM-dd"),
                    Revenue = p.CashPaid,
                    Expense = 0,
                    Description = $"مرتجع شراء {p.Id} المورد: {p.Supplier.Name}",
                    User = p.User.UserName ?? "غير معروف"
                });

            var clientReceipts = context.clientReceipts
                .Where(c => c.Date >= startDate && c.Date <= endDate && c.MoneyDrawer == moneyDrawer)
                .Select(c => new
                {
                    Time = c.Date.ToString("hh:mm tt"),
                    Date = c.Date.ToString("yyyy-MM-dd"),
                    Revenue = c.Type == "استلام" ? c.AmountPaid : 0,
                    Expense = c.Type == "صرف" ? c.AmountPaid : 0,
                    Description = $"سند ايصال مورد {c.Id} العميل: {c.Client.Name}",
                    User = c.User.UserName ?? "غير معروف"
                });

            var supplierReceipts = context.supplierReceipts
                .Where(s => s.Date >= startDate && s.Date <= endDate && s.MoneyDrawer == moneyDrawer)
                .Select(s => new
                {
                    Time = s.Date.ToString("hh:mm tt"),
                    Date = s.Date.ToString("yyyy-MM-dd"),
                    Revenue = s.Type == "استلام" ? s.AmountPaid : 0,
                    Expense = s.Type == "صرف" ? s.AmountPaid : 0,
                    Description = $"سند ايصال مورد {s.Id} المورد: {s.Supplier.Name}",
                    User = s.User.UserName ?? "غير معروف"
                });

            
            var transfers = context.Transfers
                .Where(t => t.Date >= startDate && t.Date <= endDate &&
                           (t.FromMoneyDrawer.Name == moneyDrawer || t.ToMoneyDrawer.Name == moneyDrawer))
                .Select(t => new
                {
                    Time = t.Date.ToString("hh:mm tt"),
                    Date = t.Date.ToString("yyyy-MM-dd"),
                    Revenue = t.ToMoneyDrawer.Name == moneyDrawer ? t.Amount : 0,
                    Expense = t.FromMoneyDrawer.Name == moneyDrawer ? t.Amount : 0,
                    Description = t.FromMoneyDrawer.Name == moneyDrawer
                                  ? $"تحويل من الخزينة {t.FromMoneyDrawer.Name} \n"+
                                  $" إلى الخزينة {t.ToMoneyDrawer.Name}"
    
                                  : $"تحويل إلى الخزينة {t.ToMoneyDrawer.Name} \n"+
                                   $" من الخزينة {t.FromMoneyDrawer.Name}",
                    User = t.User.UserName ?? "غير معروف"
                });
            var salaryRecords = context.totalSalaryRecords
                .Where(sr => sr.Date >= startDate && sr.Date <= endDate && sr.MoneyDrawer == moneyDrawer)
                .Select(sr => new
                {
                    Time = sr.Date.ToString("hh:mm tt"),
                    Date = sr.Date.ToString("yyyy-MM-dd"),
                    Revenue = 0,
                    Expense = sr.TotalNetSalary,
                    Description = $"صرف مرتبات لشهر {sr.Date.ToString("MMMM yyyy")}",
                    User = sr.User.UserName ?? "غير معروف"
                });
            var expenses = context.expense
        .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate && e.MoneyDrawer.Name == moneyDrawer)
        .Select(e => new
        {
            Time = e.ExpenseDate.Value.ToString("hh:mm tt"),
            Date = e.ExpenseDate.Value.ToString("yyyy-MM-dd"),
            Revenue = 0,
            Expense = e.Amount,
            Description = $"مصروف: {e.Item}",
            User = e.User.UserName ?? "غير معروف"
        });

            var buyOperations = buyBills.ToList();
            var salesOperations = salesBills.ToList();
            var salesReturnOperations = salesReturnBills.ToList();
            var purchaseReturnOperations = purchaseReturnBills.ToList();
            var clientReceiptOperations = clientReceipts.ToList();
            var supplierReceiptOperations = supplierReceipts.ToList();
            var transferOperations = transfers.ToList();
            var salaryOperations = salaryRecords.ToList();
            var expenseOperations = expenses.ToList();

            var allOperations = buyOperations
                .Select(b => new OperationType
                {
                    Date = b.Date,
                    Time = b.Time,
                    Revenue = b.Revenue,
                    Expense = b.Expense,
                    Description = b.Description,
                    User = b.User
                })
                .Concat(salesOperations.Select(s => new OperationType
                {
                    Date = s.Date,
                    Time = s.Time,
                    Revenue = s.Revenue,
                    Expense = s.Expense,
                    Description = s.Description,
                    User = s.User
                }))
                .Concat(salesReturnOperations.Select(sr => new OperationType
                {
                    Date = sr.Date,
                    Time = sr.Time,
                    Revenue = sr.Revenue,
                    Expense = sr.Expense,
                    Description = sr.Description,
                    User = sr.User
                }))
                .Concat(purchaseReturnOperations.Select(pr => new OperationType
                {
                    Date = pr.Date,
                    Time = pr.Time,
                    Revenue = pr.Revenue,
                    Expense = pr.Expense,
                    Description = pr.Description,
                    User = pr.User
                }))
                .Concat(clientReceiptOperations.Select(cr => new OperationType
                {
                    Date = cr.Date,
                    Time = cr.Time,
                    Revenue = cr.Revenue,
                    Expense = cr.Expense,
                    Description = cr.Description,
                    User = cr.User
                }))
                .Concat(supplierReceiptOperations.Select(sr => new OperationType
                {
                    Date = sr.Date,
                    Time = sr.Time,
                    Revenue = sr.Revenue,
                    Expense = sr.Expense,
                    Description = sr.Description,
                    User = sr.User
                }))
                .Concat(transferOperations.Select(tr => new OperationType
                {
                    Date = tr.Date,
                    Time = tr.Time,
                    Revenue = tr.Revenue,
                    Expense = tr.Expense,
                    Description = tr.Description,
                    User = tr.User
                }))
                .Concat(salaryOperations.Select(sr => new OperationType
                {
                    Date = sr.Date,
                    Time = sr.Time,
                    Revenue = sr.Revenue,
                    Expense = sr.Expense,
                    Description = sr.Description,
                    User = sr.User
                }))
                .Concat(expenseOperations.Select(e => new OperationType
                {
                    Date = e.Date,
                    Time = e.Time,
                    Revenue = e.Revenue,
                    Expense = e.Expense,
                    Description = e.Description,
                    User = e.User
                }))
                .OrderBy(o => o.Date)
                .ThenBy(o => o.Time)
                .ToList();

            return Json(allOperations);
        }
        [Permission("عرض صافي الربح")]
        public IActionResult ViewNetProfit()
        {
            var drawers = context.moneyDrawer.Select(c => c.Name).ToList();
            return View(drawers);
        }
        [HttpGet]
        public async Task<IActionResult> GetNetProfitReport(DateTime startDate, DateTime endDate, string moneyDrawer)
        {
            
            var salesBillsQuery = context.salesBill
                .Where(bill => bill.Date >= startDate && bill.Date <= endDate);

            var salesReturnBillsQuery = context.salesReturnBills
                .Where(bill => bill.Date >= startDate && bill.Date <= endDate);

            var buyBillsQuery = context.buyBill
                .Where(bill => bill.Date >= startDate && bill.Date <= endDate);

            var purchaseReturnBillsQuery = context.purchaseReturnBills
                .Where(bill => bill.Date >= startDate && bill.Date <= endDate);

            var expensesQuery = context.expense
                .Where(exp => exp.ExpenseDate >= startDate && exp.ExpenseDate <= endDate);

            var clientReceipts = context.clientReceipts.
                Where(x=>x.Date>= startDate && x.Date <= endDate);  

            var supplierReceipts = context.supplierReceipts.
                Where(x=>x.Date>= startDate && x.Date <= endDate);  
            var expenses = context.expense
                .Where(x=>x.ExpenseDate>= startDate && x.ExpenseDate <= endDate);

            if (!string.IsNullOrEmpty(moneyDrawer) && moneyDrawer != "all")
            {
                salesBillsQuery = salesBillsQuery.Where(bill => bill.MoneyDrawer == moneyDrawer);
                salesReturnBillsQuery = salesReturnBillsQuery.Where(bill => bill.MoneyDrawer == moneyDrawer);
                buyBillsQuery = buyBillsQuery.Where(bill => bill.MoneyDrawer == moneyDrawer);
                purchaseReturnBillsQuery = purchaseReturnBillsQuery.Where(bill => bill.MoneyDrawer == moneyDrawer);
                expensesQuery = expensesQuery.Where(exp => exp.MoneyDrawer.Name == moneyDrawer);
                clientReceipts = clientReceipts.Where(exp => exp.MoneyDrawer == moneyDrawer);
                supplierReceipts = supplierReceipts.Where(exp => exp.MoneyDrawer == moneyDrawer);
                expenses = expenses.Where(x => x.MoneyDrawer.Name == moneyDrawer);
                
            }

            var recieptsFromClients = clientReceipts.Where(r => r.Type == "استلام");
            var recieptsToClients = clientReceipts.Where(r => r.Type == "صرف");
            var recieptsToSuupliers = supplierReceipts.Where(r => r.Type == "صرف");
            var recieptsFromSuupliers = supplierReceipts.Where(r => r.Type == "استلام");


            var totalRevenue = await salesBillsQuery.SumAsync(bill => bill.CashPaid)
                + await recieptsFromClients.SumAsync(r => r.AmountPaid)
                + await recieptsFromSuupliers.SumAsync(r=>r.AmountPaid)
                              - await salesReturnBillsQuery.SumAsync(bill => bill.CashPaid);

            var totalPurchases = await buyBillsQuery.SumAsync(bill => bill.CashPaid)
                 + await recieptsToClients.SumAsync(r => r.AmountPaid)
                + await recieptsToSuupliers.SumAsync(r => r.AmountPaid)
                + await expenses.SumAsync(e => e.Amount)
                               - await purchaseReturnBillsQuery.SumAsync(bill => bill.CashPaid);

            var totalExpenses = await expensesQuery.SumAsync(exp => exp.Amount);

            var netProfit = totalRevenue - totalPurchases - totalExpenses;

            
            return Json(new
            {
                TotalRevenue = totalRevenue,
                TotalPurchases = totalPurchases,
                TotalExpenses = totalExpenses,
                NetProfit = netProfit
            });
        }

        [Permission("عرض الموظفين")]
        [HttpGet]
        public IActionResult ViewEmployees()
        {
            var model = new EmployeeViewModel
            {

                Employees = context.employees.ToList(),
                TotalSalaries = context.employees.Sum(x => x.Salary)

            };
            return View(model);
        }

        [Permission("إضافة موظف")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEmployee(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isfound = context.employees.FirstOrDefault(x => x.Name == model.Name);
                if (isfound == null)
                {
                    var emp = new Employee
                    {
                        Name = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        Job = model.Job,
                        Salary = model.Salary,
                        SalesRatio = model.SalesRatio
                    };
                    context.employees.Add(emp);
                }
                else
                {
                    isfound.Name=model.Name;
                    isfound.PhoneNumber=model.PhoneNumber;
                    isfound.Salary = model.Salary;
                    isfound.Job = model.Job;
                    isfound.SalesRatio = model.SalesRatio;
                }
                context.SaveChanges();
                
            }
            return RedirectToAction("ViewEmployees", "Admin");
        }
        [Permission("حذف موظف")]
        public IActionResult DeleteEmployee(int id)
        {
            var emp = context.employees.FirstOrDefault(x=>x.Id== id);
            if(emp == null)
            {
                return NotFound("هذا الموظف غير موجود");
            }
            context.employees.Remove(emp);
            context.SaveChanges();
            return RedirectToAction("ViewEmployees");
        }
        [Permission("عرض السلف والحوافز والخصومات")]
        public IActionResult ViewAdvancesIncentivesAndDiscounts()
        {
            var model = new NetEmpSalaryViewModel
            {
                empSalaries = context.netEmpSalaries.Include(c=>c.Employee).ToList(),
                Darwers = context.moneyDrawer.Select(c => c.Name).ToList()
        };
            return View(model);
        }
        [HttpGet]
        public JsonResult EmployeeSearch(string searchTerm)
        {
            
            var employees = context.employees
                .Where(e => e.Name.Contains(searchTerm))
                .Select(e => new
                {
                    e.Id,
                    e.Name
                })
                .ToList();

            return Json(employees);
        }

        [HttpPost]
        public IActionResult SaveItems(NetEmpSalaryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var item = new NetEmpSalary
                {
                    EmployeeId = model.EmployeeId,
                    Date = model.Date,
                    Item = model.Item,
                    Amount = model.Amount,
                                  };
                if(model.Item == "سلفة")
                {
                    var drawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                    drawer.CurrentBalance -= model.Amount;
                    item.MoneyDrawer = model.MoneyDrawer;
                }
                context.netEmpSalaries.Add(item);
                context.SaveChanges();
            }
            return RedirectToAction("ViewAdvancesIncentivesAndDiscounts");
        }
        public IActionResult DeleteItem(int id)
        {
            var item = context.netEmpSalaries.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            context.netEmpSalaries.Remove(item);
            context.SaveChanges();
            return RedirectToAction("ViewAdvancesIncentivesAndDiscounts");
        }
        [Permission("عرض السلف والحوافز والخصومات")]
        public IActionResult PayingSalaries()
        {
            // Get all employees
            var employees = context.employees.ToList();

            // Retrieve employee salary data
            var employeeSalaries = employees.Select(employee =>
            {
                // Get net salary records for the current month
                var netEmpSalaries = context.netEmpSalaries
                    .Where(n => n.EmployeeId == employee.Id && n.Date.Month == DateTime.Now.Month && n.Date.Year == DateTime.Now.Year)
                    .ToList();

                // Calculate incentive, deduction, and advance
                var incentive = netEmpSalaries
                    .Where(s => s.Item == "حافز")
                    .Sum(s => s.Amount);
                var deduction = netEmpSalaries
                    .Where(s => s.Item == "خصم")
                    .Sum(s => s.Amount);
                var advance = netEmpSalaries
                    .Where(s => s.Item == "سلفة")
                    .Sum(s => s.Amount);

                // Fetch the total sales for the current month by this employee
                var totalSales = context.salesBill
                    .Where(s => s.EmployeeId == employee.Id && s.Date.Month == DateTime.Now.Month && s.Date.Year == DateTime.Now.Year)
                    .Sum(s => s.TotalAmount);

                // Get the sales commission ratio for the employee (assuming it's stored in the Employee table)
                var salesRatio = employee.SalesRatio;

                // Calculate the commission
                var commission = totalSales * (salesRatio / 100);

                // Calculate the net salary (fixed salary + commission - deductions and advances)
                var netSalary = employee.Salary + commission + incentive - deduction - advance;

                // Return the employee's salary data
                return new EmployeeSalaryViewModel
                {
                    EmployeeId = employee.Id,
                    Name = employee.Name,
                    Job = employee.Job,
                    BaseSalary = employee.Salary,
                    Incentive = incentive,
                    Deduction = deduction,
                    Advance = advance,
                    
                    TotalSales = totalSales,
                    NetSalary = netSalary
                };
            }).ToList();

            // Retrieve money drawer names
            var moneyDrawerNames = context.moneyDrawer.Select(md => md.Name).ToList();

            // Create the view model to pass to the view
            var viewModel = new PayingSalariesViewModel
            {
                Employees = employeeSalaries,
                MoneyDrawers = moneyDrawerNames
            };

            return View(viewModel);
        }
        [Permission("عرض السلف والحوافز والخصومات")]
        [HttpPost]
        public IActionResult SaveTotalSalary(string SelectedMoneyDrawer, decimal TotalNetSalary)
        {
            
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            bool salaryAlreadyPaid = context.totalSalaryRecords
                .Any(record => record.Date.Month == currentMonth && record.Date.Year == currentYear);

            if (salaryAlreadyPaid)
            {
                return BadRequest("المرتبات لهذا الشهر قد تم دفعها بالفعل.");
            }

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var salaryRecord = new TotalSalaryRecord
            {
                MoneyDrawer = SelectedMoneyDrawer,
                TotalNetSalary = TotalNetSalary,
                UserId = userId,
                Date = DateTime.Now
            };

            var drawer = context.moneyDrawer.FirstOrDefault(x => x.Name == SelectedMoneyDrawer);

            if (drawer != null && drawer.CurrentBalance >= TotalNetSalary)
            {
                drawer.CurrentBalance -= TotalNetSalary;
                context.totalSalaryRecords.Add(salaryRecord);
                context.SaveChanges();

                return RedirectToAction("PayingSalaries");
            }

            return BadRequest("رصيد الخزنة غير كافي للمرتبات");
        }


    }
}
