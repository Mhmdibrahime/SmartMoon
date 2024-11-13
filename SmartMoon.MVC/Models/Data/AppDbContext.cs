using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartMoon.MVC.Models.Data.Configurations;
using SmartMoon.MVC.Models.Entities;
using System.Security.Cryptography.Xml;

namespace SmartMoon.MVC.Models.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }
        
        public DbSet<ApplicationUser> users { get; set; }
        public DbSet<UserPermission> permissions { get; set; }
        public DbSet<Client> clients { get; set; }
        public DbSet<Supplier> suppliers { get; set; }
        public DbSet<SalesReturnBill> salesReturnBills { get; set; }
        public DbSet<SalesBill> salesBill { get; set; }
        public DbSet<SalesReturnBillItem> salesReturnBillItems { get; set; }
        public DbSet<SalesBillItem> SalesBillItem { get; set; }
        public DbSet<BuyBill> buyBill { get; set; } 
        public DbSet<PurchaseReturnBill> purchaseReturnBills { get; set; } 
        public DbSet<BuyBillItem> buyBillItems { get; set; }
        public DbSet<PurchaseReturnBillItem> purchaseReturnBillItems { get; set; }
        public DbSet<Inventory> inventories { get; set; } 
        public DbSet<MoneyDrawer> moneyDrawer { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductSupplier> productSuppliers { get; set; }
        public DbSet<InventoryProduct> inventoryProducts { get; set; }
        public DbSet<Expense> expense { get; set; }
        public DbSet<ClientReceipt> clientReceipts { get; set; }
        public DbSet<SupplierReceipt> supplierReceipts { get; set; }
        public DbSet<InventoryProductBatch> inventoryProductBatches { get; set; }
        public DbSet<ProductBatch> productBatches { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<NetEmpSalary> netEmpSalaries { get; set; }
        public DbSet<TotalSalaryRecord> totalSalaryRecords { get; set; }
        public DbSet<TransferBetweenMoneyDrawers> Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ProductConfig).Assembly);

        }
    }
}
