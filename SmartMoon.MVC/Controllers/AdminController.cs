using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
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
        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var product = context.products.FirstOrDefault(p => p.Id == id);
            if(product != null)
            {
                context.products.Remove(product);
                context.SaveChanges();
            }
            return RedirectToAction("ViewProductsShortcomings");
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
                // Calculate total amount for the purchase bill
                model.TotalAmount = model.Items.Sum(item => item.PurchasePrice * item.Quantity);

                // Calculate remaining balance
                model.RemainingBalance = model.TotalAmount - model.DiscountAmount - model.CashPaid;

                // Create BuyBill entity
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
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x=>x.Name==model.MoneyDrawer);
                moneyDrawer.CurrentBalance -= model.TotalAmount;
                context.moneyDrawer.Update(moneyDrawer);
                foreach (var item in model.Items)
                {
                    // Update the total quantity in the Products table
                    var product = context.products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity; // Increase the total product quantity
                        product.Price = item.SalePrice; // Update the sale price
                        context.products.Update(product);
                    }

                    // Update the quantity in the InventoryProduct table
                    var inventoryProduct = context.inventoryProducts
                        .FirstOrDefault(ip => ip.ProductId == item.ProductId && ip.InventoryId == item.InventoryId);

                    if (inventoryProduct != null)
                    {
                        // If the product already exists in this inventory, update the quantity
                        inventoryProduct.Quantity += item.Quantity;
                        context.inventoryProducts.Update(inventoryProduct);
                    }
                    else
                    {
                        // If the product is not in this inventory, create a new InventoryProduct record
                        var newInventoryProduct = new InventoryProduct
                        {
                            ProductId = item.ProductId,
                            InventoryId = item.InventoryId,
                            Quantity = item.Quantity
                        };
                        context.inventoryProducts.Add(newInventoryProduct);
                    }
                }

                // Save the bill and inventory changes to the database
                context.buyBill.Add(bill);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            // If model state is invalid, reload dropdowns for the view
            model.Suppliers = context.suppliers.ToList();
            model.Products = context.products.ToList();
            model.Inventories = context.inventories.ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateSalesBill()
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
                    Items = model.Items.Select(i => new SalesBillItem
                    {
                        InventoryId = i.InventoryId,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        SalePrice = i.SalePrice,
                        TotalPrice = i.TotalPrice
                    }).ToList()
                };
                var client = context.clients.FirstOrDefault(x => x.Id == model.ClientId);
                if(client != null && model.RemainingBalance != 0)
                {
                    client.Balance -= model.RemainingBalance;
                }
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                moneyDrawer.CurrentBalance += model.TotalAmount;
                context.moneyDrawer.Update(moneyDrawer);
                foreach (var item in model.Items)
                {
                    // Update the total quantity in the Products table
                    var product = context.products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity; // Increase the total product quantity
                         
                        context.products.Update(product);
                    }

                    // Update the quantity in the InventoryProduct table
                    var inventoryProduct = context.inventoryProducts
                        .FirstOrDefault(ip => ip.ProductId == item.ProductId && ip.InventoryId == item.InventoryId);

                    if (inventoryProduct != null)
                    {
                        // If the product already exists in this inventory, update the quantity
                        inventoryProduct.Quantity -= item.Quantity;
                        context.inventoryProducts.Update(inventoryProduct);
                    }
                   
                }

                context.salesBill.Add(salesBill);
                context.SaveChanges();

                return RedirectToAction("Index","Home");
            }

            // Reload customers and money drawers if validation fails
            model.clients = context.clients.ToList();
            model.MoneyDrawers = context.moneyDrawer.ToList();
            model.inventories = context.inventories.ToList();
               model.products = context.products.ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult GetAvailableStock(int productId, int inventoryId)
        {

            var availableQuantity = context.inventoryProducts
                           .Where(ip => ip.ProductId == productId && ip.InventoryId == inventoryId)
                           .Select(ip => ip.Quantity)
                           .FirstOrDefault();


            if(availableQuantity > 0) return Json(new { availableQuantity = availableQuantity });
            
            else return Json(new { availableQuantity = 0 });
        }

        [HttpGet]
        public IActionResult CreatePurchaseReturnBill()
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
        public IActionResult CreatePurchaseReturnBill(PurchaseBillViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Calculate total amount for the purchase bill
                model.TotalAmount = model.Items.Sum(item => item.PurchasePrice * item.Quantity);

                // Calculate remaining balance
                model.RemainingBalance = model.TotalAmount - model.DiscountAmount - model.CashPaid;

                // Create BuyBill entity
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
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                moneyDrawer.CurrentBalance += model.TotalAmount;
                context.moneyDrawer.Update(moneyDrawer);
                foreach (var item in model.Items)
                {
                    // Update the total quantity in the Products table
                    var product = context.products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity; // Increase the total product quantity
                         
                        context.products.Update(product);
                    }

                    // Update the quantity in the InventoryProduct table
                    var inventoryProduct = context.inventoryProducts
                        .FirstOrDefault(ip => ip.ProductId == item.ProductId && ip.InventoryId == item.InventoryId);

                    if (inventoryProduct != null)
                    {
                        // If the product already exists in this inventory, update the quantity
                        inventoryProduct.Quantity -= item.Quantity;
                        context.inventoryProducts.Update(inventoryProduct);
                    }
                   
                }

                // Save the bill and inventory changes to the database
                context.purchaseReturnBills.Add(bill);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            // If model state is invalid, reload dropdowns for the view
            model.Suppliers = context.suppliers.ToList();
            model.Products = context.products.ToList();
            model.Inventories = context.inventories.ToList();

            return View(model);
        }
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
                var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Name == model.MoneyDrawer);
                moneyDrawer.CurrentBalance -= model.TotalAmount;
                context.moneyDrawer.Update(moneyDrawer);
                foreach (var item in model.Items)
                {
                    // Update the total quantity in the Products table
                    var product = context.products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity; // Increase the total product quantity

                        context.products.Update(product);
                    }

                    // Update the quantity in the InventoryProduct table
                    var inventoryProduct = context.inventoryProducts
                        .FirstOrDefault(ip => ip.ProductId == item.ProductId && ip.InventoryId == item.InventoryId);

                    if (inventoryProduct != null)
                    {
                        // If the product already exists in this inventory, update the quantity
                        inventoryProduct.Quantity += item.Quantity;
                        context.inventoryProducts.Update(inventoryProduct);
                    }

                }

                context.salesReturnBills.Add(salesBill);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            // Reload customers and money drawers if validation fails
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
            expense.Expenses = context.expense.ToList();
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
                    var expense = context.expense.FirstOrDefault(x => x.Item == Expense.Item);
                    expense.Amount = Expense.Amount;
                    expense.MoneyDrawerId = Expense.MoneyDrawerId;
                    context.expense.Update(expense);
                    var moneyDrawer = context.moneyDrawer.FirstOrDefault(x => x.Id == expense.MoneyDrawerId);
                    moneyDrawer.CurrentBalance -= expense.Amount;
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
                products = context.products.Where(x=>x.Quantity>0).ToList()
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

                var toInventoryProduct = context.inventoryProducts
                    .FirstOrDefault(ip => ip.InventoryId == toInventoryId && ip.ProductId == transfer.ProductId);

                if (fromInventoryProduct == null || fromInventoryProduct.Quantity < transfer.Quantity)
                    return BadRequest("Insufficient quantity for transfer.");

                // Deduct from source inventory
                fromInventoryProduct.Quantity -= transfer.Quantity;

                // Add to target inventory
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
                        PaymentMethod = "نقدى", // Assuming cash, adapt as needed
                        SalesPoint = sb.MoneyDrawer,
                        SalesPerson = "سامى" // Assuming static salesperson, adapt as needed
                    }).ToList()
            };

            // Calculate the totals
            viewModel.TotalAmount = viewModel.SalesBills.Sum(b => b.TotalPrice);
            viewModel.TotalPaid = viewModel.Receipts.Sum(r => r.AmountPaid);
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
            viewModel.TotalPaid = viewModel.Receipts.Sum(r => r.AmountPaid);
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
            var products = context.products.Where(x=>x.Quantity==0).ToList();
            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> LateInstallments()
        {
            int overdueThresholdDays = 30; // Define what counts as "late" (e.g., 30 days overdue)
            DateTime currentDate = DateTime.Now;


            var lateInstallments = context.salesBill
                .Where(bill => bill.RemainingBalance > 0)
                .AsEnumerable()
                .Where(bill => (currentDate - bill.Date).TotalDays > overdueThresholdDays)
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
        public IActionResult FinancialPosition()
        {
            var forClients = context.clients.Where(x=>x.Balance > 0).Sum(x=>x.Balance);
            var onClients = context.clients.Where(x=>x.Balance < 0).Sum(x=>x.Balance);
            var onSuppliers = context.suppliers.Where(x=>x.Balance < 0).Sum(x=>x.Balance);
            var forSuppliers = context.suppliers.Where(x=>x.Balance > 0).Sum(x=>x.Balance);
            var drawersBalance = context.moneyDrawer.Sum(x => x.CurrentBalance);
            var inventoriesBalance =  context.inventoryProducts.Sum(ip => ip.Quantity * ip.Product.Price);
            var model = new FinancialPositionViewModel
            {
                ForClients = forClients,
                OnClients = Math.Abs(onClients),
                OnSuppliers = Math.Abs(onSuppliers),
                ForSuppliers = forSuppliers,
                DrawersActualBalance = drawersBalance,
                InventoriesActualBalance = inventoriesBalance,
                ActualTotalBalance = drawersBalance+inventoriesBalance,
                TotalForUs = Math.Abs(onClients) + Math.Abs(onSuppliers),
                TotalOnUs = forClients+forSuppliers,
                
            };
            model.FinalPosition = (model.ActualTotalBalance + model.TotalForUs) - model.TotalOnUs;
            return View(model);
        }
    }
}
