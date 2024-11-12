using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using SmartMoon.MVC.Models.CustomAuthorization;
using SmartMoon.MVC.Models.Data;
using SmartMoon.MVC.Models.Entities;
using SmartMoon.MVC.Models.ViewModels;
using System.Drawing;
using System.Linq;

namespace SmartMoon.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext context;

        public AdminController(AppDbContext context)
        {
            this.context = context;
        }

        //[AuthorizePermission("إضافة عميل")]
        public ActionResult AddClient() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AuthorizePermission("إضافة عميل")]
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
                    return RedirectToAction("index", "home");
                }
                catch
                {
                    throw new Exception("!بيانات المورد المدخله ليست صالحه");
                }  
            }
            return View(model);
        }

        // View products and their suppliers
        public IActionResult ViewProducts()
        {
            var productsWithSuppliers = context.products
                .Where(x=>x.Quantity > 0)
                .Include(x => x.productSuppliers)
                .ThenInclude(x => x.Supplier)
                .ToList();

            var products = productsWithSuppliers.Select(p => new ProductsViewModel
            {
                ProductName = p.Name,
                Price = p.Price,
                Quantity = p.Quantity,
                SuppliersName = p.productSuppliers.Select(ps => ps.Supplier.Name).ToList()
            }).ToList();

            var model = new ViewProductsWithSuppliersViewModel
            {
                Products = products,
                Suppliers = context.suppliers.ToList()
            };

            return View(model);
        }

        // Add a new product
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

        // Delete a product and its related records
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

        //[AuthorizePermission("إنشاء فاتورة مبيعات")]
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

        //[AuthorizePermission("إنشاء فاتورة مبيعات")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CreatePurchaseBill(PurchaseBillViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.TotalAmount = model.Items.Sum(item => item.PurchasePrice * item.Quantity);
                model.RemainingBalance = model.TotalAmount - model.DiscountAmount - model.CashPaid;

                var bill = new BuyBill
                {
                    SupplierId = model.SupplierId,
                    PaymentMethod = model.PaymentMethod,
                    DiscountAmount = model.DiscountAmount,
                    CashPaid = model.CashPaid,
                    RemainingBalance =model.RemainingBalance,
                    MoneyDrawer = model.MoneyDrawer,
                    TotalAmount = model.TotalAmount,
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
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                if (moneyDrawer != null)
                {
                    moneyDrawer.CurrentBalance -= model.CashPaid;
                    context.moneyDrawer.Update(moneyDrawer);
                }

                foreach (var item in model.Items)
                {
                    var product = context.products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (product != null)
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

                    // Create or update a batch for each item
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

        // GET: Create sales bill
        // GET: Create sales bill
        [HttpGet]
        public IActionResult CreateSalesBill()
        {
            var model = new SalesBillViewModel
            {
                clients = context.clients.ToList(),
                MoneyDrawers = context.moneyDrawer.ToList(),
                inventories = context.inventories.ToList(),
                products = context.products.Where(p => p.Quantity > 0).ToList(),
                Items = new List<SalesBillItemViewModel>()
            };

            return View(model);
        }

        // POST: Create sales bill and update inventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSalesBill(SalesBillViewModel model)
        {
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
                    Items = new List<SalesBillItem>()
                };
                 if(model.RemainingBalance > 0)
                {
                    var client = context.clients.FirstOrDefault(x => x.Id == model.ClientId);
                    if(client != null)
                    {
                        client.Balance -= model.RemainingBalance;
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
                        return BadRequest("Product not available or insufficient quantity.");
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

                // Save the sales bill and apply all inventory changes
                context.salesBill.Add(salesBill);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            // Reload data if model state is invalid
            model.clients = context.clients.ToList();
            model.MoneyDrawers = context.moneyDrawer.ToList();
            model.inventories = context.inventories.ToList();
            model.products = context.products.ToList();

            return View(model);
        }


        // GET: Check available stock
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
                                      .Where(pb => pb.ProductId == productId)
                                      .OrderByDescending(pb => pb.PurchaseDate)
                                      .FirstOrDefault();

            if (latestBatch != null)
            {
                return Json(new { purchasePrice = latestBatch.PurchasePrice });
            }
            return Json(new { purchasePrice = 0 });
        }


        // GET: Create purchase return bill
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

        // POST: Create purchase return bill and update inventory
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CreatePurchaseReturnBill(PurchaseBillViewModel model)
        {
            if (ModelState.IsValid)
            {
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
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                if (moneyDrawer != null)
                {
                    moneyDrawer.CurrentBalance += model.TotalAmount;
                    context.moneyDrawer.Update(moneyDrawer);
                }

                foreach (var item in model.Items)
                {
                    var quantityToReturn = item.Quantity;

                    var batch = context.productBatches
                        .FirstOrDefault(pb => pb.ProductId == item.ProductId &&
                                              pb.InventoryId == item.InventoryId &&
                                              pb.PurchasePrice == item.PurchasePrice);

                    if (batch != null)
                    {
                        batch.Quantity += quantityToReturn;
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

        // GET: Create return sales bill
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

        // POST: Create return sales bill and update inventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateReturnSalesBill(SalesBillViewModel model)
        {
            if (ModelState.IsValid)
            {
                var salesBill = new SalesReturnBill
                {
                    ClientId = model.ClientId,
                    TotalAmount = model.TotalAmount,
                    DiscountAmount = model.DiscountAmount,
                    CashPaid = model.CashPaid,
                    RemainingBalance = model.RemainingBalance,
                    MoneyDrawer = model.MoneyDrawer,
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
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                if (moneyDrawer != null)
                {
                    moneyDrawer.CurrentBalance -= model.TotalAmount;
                    context.moneyDrawer.Update(moneyDrawer);
                }

                foreach (var item in model.Items)
                {
                    var quantityToReturn = item.Quantity;

                    var batches = context.productBatches
                        .Where(pb => pb.ProductId == item.ProductId && pb.InventoryId == item.InventoryId)
                        .OrderByDescending(pb => pb.PurchaseDate)
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
        [HttpGet]
        public IActionResult AddExpense()
        {
            var expense = new ExpenseViewModel();
            expense.Expenses = context.expense.Where(x => x.Amount > 0).ToList();
            expense.moneyDrawers=context.moneyDrawer.ToList();
            return View(expense);
        }
        [HttpPost]
        public IActionResult AddNewExpense(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {

                var expense = new Expense();
                expense.Item = model.Item;
                expense.Amount = 0;
                expense.MoneyDrawerId = 1;
                context.expense.Add(expense);
                context.SaveChanges();
                return RedirectToAction("AddExpense");
            }
            return View(model);
        }
        public IActionResult AddExpense(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var Expense in model.Expenses)
                {
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
        [HttpGet]
        public IActionResult TransferBetweenMoneyDrawers()
        {
            var model = new TransferMoneyViewModel
            {
                moneyDrawers = context.moneyDrawer.ToList()
            };
            return View(model);
        }
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
        [HttpPost]
        public IActionResult SaveTransfer(TransferMoneyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var from = context.moneyDrawer.FirstOrDefault(x => x.Id == model.FromId);
                var to = context.moneyDrawer.FirstOrDefault(y => y.Id == model.ToId);
                if(from !=  null && to != null)
                {
                    from.CurrentBalance -= model.TotalAmount;
                    to.CurrentBalance += model.TotalAmount;
                    context.moneyDrawer.Update(from);
                    context.moneyDrawer.Update(to);
                }
                context.SaveChanges();
                return RedirectToAction("TransferBetweenMoneyDrawers", "Admin");
            }
            return View("TransferBetweenMoneyDrawers", model);
        }

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
        [HttpGet]
        public IActionResult AddFromClientReceipt()
        {
            
           

            var viewModel = new ReceiptViewModel
            {
                
                moneyDrawers = context.moneyDrawer.ToList()
            };

            return View(viewModel);
        }

        // GET: Get client's previous balance via AJAX
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

        // POST: Process the receipt form submission
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

                var receipt = new ClientReceipt
                {
                    ClientId = model.Id,
                    AmountPaid = model.PaymentAmount,
                    Type = "استلام",
                    MoneyDrawer = moneyDrawer.Name,
                    Date = DateTime.Now
                };

                context.clientReceipts.Add(receipt);
                context.SaveChanges();

                TempData["Message"] = "!تم حفظ الايصال بنجاح";
                return RedirectToAction("Index", "Home");
            }
            model.moneyDrawers = context.moneyDrawer.ToList();
            return View("AddFromClientReceipt", model);
        }
        [HttpGet]
        public IActionResult AddToClientReceipt()
        {



            var viewModel = new ReceiptViewModel
            {
                
                moneyDrawers = context.moneyDrawer.ToList()
            };

            return View(viewModel);
        }
        // POST: Process the receipt form submission
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

                var receipt = new ClientReceipt
                {
                    ClientId = model.Id,
                    AmountPaid = model.PaymentAmount,
                    Type = "صرف",
                    MoneyDrawer = moneyDrawer.Name,
                    Date = DateTime.Now
                };

                context.clientReceipts.Add(receipt);
                context.SaveChanges();

                TempData["Message"] = "!تم حفظ الايصال بنجاح";
                return RedirectToAction("Index", "Home");
            }
            model.moneyDrawers = context.moneyDrawer.ToList();
            return View("AddToClientReceipt", model);
        }
        public IActionResult ViewClients()
        {
            var clients = context.clients.ToList();
            return View(clients);
        }
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
                        PaymentMethod = "نقدى", 
                        SalesPoint = sb.MoneyDrawer,
                        SalesPerson = "سامى" 
                    }).ToList()
            };

            
            viewModel.TotalAmount = viewModel.SalesBills.Sum(b => b.TotalPrice);
            viewModel.TotalPaid = viewModel.Receipts.Sum(r => r.AmountPaid)+context.salesBill.Where(x=>x.ClientId==clientId).Sum(x=>x.CashPaid);
            viewModel.TotalRemaining = viewModel.TotalAmount - viewModel.TotalPaid;

            return View("AccountStatement", viewModel); // Ensure that the view is named AccountStatement.cshtml
        }

        // Action to view receipts and sales bills filtered by date range
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
                        PaymentMethod = "نقدى", // Assuming cash, adapt as needed
                        SalesPoint = sb.MoneyDrawer,
                        SalesPerson = "سامى" // Assuming static salesperson, adapt as needed
                    }).ToList()
            };

            // Calculate the totals
            viewModel.TotalAmount = viewModel.SalesBills.Sum(b => b.TotalPrice);
            viewModel.TotalPaid = viewModel.Receipts.Sum(r => r.AmountPaid)+context.salesBill.Where(x => x.ClientId == clientId).Sum(x => x.CashPaid);
            viewModel.TotalRemaining = viewModel.TotalAmount - viewModel.TotalPaid;

            return View("AccountStatement", viewModel); // Reuse the same view for filtered results
        }
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

                var receipt = new SupplierReceipt
                {
                    SupplierId = model.Id,
                    AmountPaid = model.PaymentAmount,
                    Type = "استلام",
                    MoneyDrawer = moneyDrawer.Name,
                    Date = DateTime.Now
                };

                context.supplierReceipts.Add(receipt);
                context.SaveChanges();

                TempData["Message"] = "!تم حفظ الايصال بنجاح";
                return RedirectToAction("Index", "Home");
            }
            model.moneyDrawers = context.moneyDrawer.ToList();
            return View("AddFromSupplierReceipt",model);
        }
        [HttpGet]
        public IActionResult AddToSupplierReceipt()
        {



            var viewModel = new ReceiptViewModel
            {

                moneyDrawers = context.moneyDrawer.ToList()
            };

            return View(viewModel);
        }
        // POST: Process the receipt form submission
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

                var receipt = new SupplierReceipt
                {
                    SupplierId = model.Id,
                    AmountPaid = model.PaymentAmount,
                    Type = "صرف",
                    MoneyDrawer = moneyDrawer.Name,
                    Date = DateTime.Now
                };

                context.supplierReceipts.Add(receipt);
                context.SaveChanges();

                TempData["Message"] = "!تم حفظ الايصال بنجاح";
                return RedirectToAction("Index", "Home");
            }
            model.moneyDrawers = context.moneyDrawer.ToList();
            return View("AddToSupplierReceipt", model);
        }
        [HttpGet]
        public IActionResult ViewSuppliers()
        {
            var suppliers = context.suppliers.ToList();
            return View(suppliers);
        }
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
                        PaymentMethod = "نقدى", // Assuming cash, adapt as needed
                        SalesPoint = sb.MoneyDrawer,
                        SalesPerson = "سامى" // Assuming static salesperson, adapt as needed
                    }).ToList(),
                TotalCashpaid = context.buyBill.Where(x => x.SupplierId == supplierId).Sum(x => x.CashPaid)
                    
            };

            // Calculate the totals
            viewModel.TotalAmount = viewModel.PurchaseBills.Sum(b => b.TotalPrice);
            viewModel.TotalPaid = viewModel.Receipts.Where(x=>x.Type=="صرف").Sum(r => r.AmountPaid)+viewModel.TotalCashpaid;
            viewModel.TotalRemaining = viewModel.TotalAmount - viewModel.TotalPaid;

            return View("SupplierAccountStatement", viewModel); // Ensure that the view is named AccountStatement.cshtml
        }

        // Action to view receipts and sales bills filtered by date range
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
                        PaymentMethod = "نقدى", // Assuming cash, adapt as needed
                        SalesPoint = sb.MoneyDrawer,
                        SalesPerson = "سامى" // Assuming static salesperson, adapt as needed
                    }).ToList()
            };

            // Calculate the totals
            viewModel.TotalAmount = viewModel.PurchaseBills.Sum(b => b.TotalPrice);
            viewModel.TotalPaid = viewModel.Receipts.Sum(r => r.AmountPaid);
            viewModel.TotalRemaining = viewModel.TotalAmount - viewModel.TotalPaid;

            return View("SupplierAccountStatement", viewModel); // Reuse the same view for filtered results
        }

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
        [HttpGet]
        public IActionResult ViewProductsShortcomings()
        {
            var products = context.products.Where(x => x.Quantity <= 0).ToList();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> LateInstallments()
        {
            DateTime currentDate = DateTime.Now;

            
            var lateInstallments =  context.salesBill
                .Include(bill => bill.client) 
                .Where(bill => bill.RemainingBalance > 0 && bill.client != null && bill.client.Balance < 0)
                .AsEnumerable()
                .GroupBy(bill => bill.ClientId)
                .Select(group => group
                    .OrderByDescending(bill => bill.Date)
                    .First())
                .Select(bill => new LateInstallmentsViewModel
                {
                    Id = bill.client.Id,
                    Name = bill.client.Name,
                    PhoneNumber = bill.client.PhoneNumber,
                    Balance = bill.client.Balance,
                    DaysLate = (int)(currentDate - bill.Date).TotalDays
                })
                .ToList();

            return View(lateInstallments);
        }





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
        [HttpGet]
        public IActionResult ItemMovement()
        {
            var drawres = context.moneyDrawer.ToList();
            return View(drawres);
        }
        [HttpGet]
        public async Task<IActionResult> GetFilteredReport(
     string productName,
    
     string moneyDrawer,
     DateTime? startDate,
     DateTime? endDate,
     string operationType)
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
                        .Select(bb => new
                        {
                            bb.Date,
                            
                            bb.MoneyDrawer,
                            ProductName = bb.BillItems.Select(bi => bi.Product.Name).FirstOrDefault(),
                            Price = bb.BillItems.Select(bi => bi.Product.Price).FirstOrDefault(),
                            Quantity = bb.BillItems.Select(i => i.Quantity),
                            Name = bb.Supplier.Name,
                            ItemCount = bb.BillItems.Count
                        });
                    break;

                case "SalesBill": // SalesBill
                    query = context.salesBill
                        .Include(sb => sb.client)
                        .Where(sb => (string.IsNullOrEmpty(productName) || sb.Items.Any(i => i.Product.Name.Contains(productName))) &&
                                     
                                     (string.IsNullOrEmpty(moneyDrawer) || sb.MoneyDrawer == moneyDrawer) &&
                                     (!startDate.HasValue || sb.Date >= startDate.Value) &&
                                     (!endDate.HasValue || sb.Date <= endDate.Value))
                        .Select(sb => new
                        {
                            sb.Date,
                           
                            sb.MoneyDrawer,

                            ProductName = sb.Items.Select(i => i.Product.Name).FirstOrDefault(),
                            Price = sb.Items.Select(i => i.Product.Price).FirstOrDefault(),
                            Quantity = sb.Items.Select(i=>i.Quantity),
                            Name = sb.client.Name,
                            ItemCount = sb.Items.Count
                        });
                    break;

                case "SalesReturnBill": // SalesReturnBill
                    query = context.salesReturnBills
                        .Include(srb => srb.client)
                        .Where(srb => (string.IsNullOrEmpty(productName) || srb.Items.Any(i => i.Product.Name.Contains(productName))) &&
                                     
                                     (string.IsNullOrEmpty(moneyDrawer) || srb.MoneyDrawer == moneyDrawer) &&
                                     (!startDate.HasValue || srb.Date >= startDate.Value) &&
                                     (!endDate.HasValue || srb.Date <= endDate.Value))
                        .Select(srb => new
                        {
                            srb.Date,
                            
                            srb.MoneyDrawer,
                            ProductName = srb.Items.Select(i => i.Product.Name).FirstOrDefault(),
                            Price = srb.Items.Select(i => i.Product.Price).FirstOrDefault(),
                            Quantity = srb.Items.Select(i => i.Quantity),
                            Name = srb.client.Name,
                            ItemCount = srb.Items.Count
                        });
                    break;

                case "PurchaseReturnBill": 
                    query = context.purchaseReturnBills
                        .Include(pr => pr.Supplier)
                        .Where(pr => (string.IsNullOrEmpty(productName) || pr.BillItems.Any(bi => bi.Product.Name.Contains(productName))) &&
                                     (string.IsNullOrEmpty(moneyDrawer) || pr.MoneyDrawer == moneyDrawer) &&
                                     (!startDate.HasValue || pr.Date >= startDate.Value) &&
                                     (!endDate.HasValue || pr.Date <= endDate.Value))
                        .Select(pr => new
                        {
                            pr.Date,
                            
                            pr.MoneyDrawer,
                            ProductName = pr.BillItems.Select(bi => bi.Product.Name).FirstOrDefault(),
                            Price = pr.BillItems.Select(bi => bi.Product.Price).FirstOrDefault(),
                            Quantity = pr.BillItems.Select(i => i.Quantity),
                            Name = pr.Supplier.Name,
                            ItemCount = pr.BillItems.Count
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
                .Select(s => new
                {
                    s.Date,
                    Items = s.Items.Select(item => new
                    {
                        item.Product.Name,
                        item.Quantity,
                        SalePrice = item.SalePrice,
                        TotalPrice = item.TotalPrice,
                        PurchasePrice = item.Product.ProductBatches
                            .Where(batch => batch.ProductId == item.ProductId)
                            .OrderBy(batch => batch.PurchaseDate) 
                            .Select(batch => batch.PurchasePrice)
                            .FirstOrDefault(),
                        Profit = item.TotalPrice - (item.Quantity * item.Product.ProductBatches
                            .Where(batch => batch.ProductId == item.ProductId)
                            .OrderBy(batch => batch.PurchaseDate)
                            .Select(batch => batch.PurchasePrice)
                            .FirstOrDefault())
                    }),
                    User = s.client.Name ?? "غير معروف" 
                })
                .ToList();

            return Json(salesData);
        }

        [HttpGet]
        public IActionResult ViewDrawerOperations()
        {
            var drawers = context.moneyDrawer.Select(x => x.Name).ToList();
            return View(drawers);
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
                    User = "المستخدم 1"
                });

            var salesBills = context.salesBill
                .Where(s => s.Date >= startDate && s.Date <= endDate && s.MoneyDrawer == moneyDrawer)
                .Select(s => new
                {
                    Time = s.Date.ToString("hh:mm tt"),
                    Date = s.Date.ToString("yyyy-MM-dd"),
                    Revenue = s.CashPaid,
                    Expense = 0,
                    Description = $"فاتورة مبيعات {s.Id} العميل: {s.client.Name}",
                    User = "المستخدم 2"
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
                    User = "المستخدم 3"
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
                    User = "المستخدم 4"
                });

            // Fetch the operations data from different sources
            var buyOperations =  buyBills.ToList();
            var salesOperations =  salesBills.ToList();
            var salesReturnOperations =  salesReturnBills.ToList();
            var purchaseReturnOperations =  purchaseReturnBills.ToList();

            // Concatenate and project the data into OperationType objects
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
                .OrderBy(o => o.Date)
                .ThenBy(o => o.Time)
                .ToList();




            return Json(allOperations);
        }

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
        public IActionResult PayingSalaries()
        {
            // Get all employees
            var employees = context.employees.ToList();

            // Retrieve employee salary data
            var employeeSalaries = employees.Select(employee =>
            {
                var netEmpSalaries = context.netEmpSalaries
                    .Where(n => n.EmployeeId == employee.Id && n.Date.Month == DateTime.Now.Month && n.Date.Year == DateTime.Now.Year)
                    .ToList();

                var incentive = netEmpSalaries
                    .Where(s => s.Item == "حافز")
                    .Sum(s => s.Amount);
                var deduction = netEmpSalaries
                    .Where(s => s.Item == "خصم")
                    .Sum(s => s.Amount);
                var advance = netEmpSalaries
                    .Where(s => s.Item == "سلفة")
                    .Sum(s => s.Amount);

                var netSalary = employee.Salary + incentive - deduction - advance;

                return new EmployeeSalaryViewModel
                {
                    EmployeeId = employee.Id,
                    Name = employee.Name,
                    Job = employee.Job,
                    BaseSalary = employee.Salary,
                    Incentive = incentive,
                    Deduction = deduction,
                    Advance = advance,
                    NetSalary = netSalary
                };
            }).ToList();

           
            var moneyDrawerNames = context.moneyDrawer.Select(md => md.Name).ToList();

            
            var viewModel = new PayingSalariesViewModel
            {
                Employees = employeeSalaries,
                MoneyDrawers = moneyDrawerNames
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SaveTotalSalary(string SelectedMoneyDrawer, decimal TotalNetSalary)
        {
            
            var salaryRecord = new TotalSalaryRecord
            {
                MoneyDrawer = SelectedMoneyDrawer,
                TotalNetSalary = TotalNetSalary,
                Date = DateTime.Now
            };
            var drawer = context.moneyDrawer.FirstOrDefault(x => x.Name == SelectedMoneyDrawer);
            if(drawer.CurrentBalance >= TotalNetSalary)
            {
                drawer.CurrentBalance -= TotalNetSalary;
                return BadRequest("رصيد الخزنة غير كافي للمرتبات");
            }
            context.totalSalaryRecords.Add(salaryRecord);
            context.SaveChanges();

            TempData["SuccessMessage"] = "تم حفظ إجمالي المرتب بنجاح";
            return RedirectToAction("PayingSalaries");
        }

    }
}
