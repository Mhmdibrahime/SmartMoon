using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartMoon.MVC.Models.Data.Configurations;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }
        
        public DbSet<ApplicationUser> users { get; set; }
        public DbSet<Client> clients { get; set; }
        public DbSet<Supplier> suppliers { get; set; }
        public DbSet<SalesBill> salesBill { get; set; }
        public DbSet<BuyBill> buyBill { get; set; } 
        public DbSet<SaleTransaction> saleTransaction { get; set; }
        public DbSet<BuyTransaction> buyTransactions { get; set; }
        public DbSet<MoneyDrawer> moneyDrawer { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductSupplier> productSuppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ProductConfig).Assembly);

        }
    }
}
