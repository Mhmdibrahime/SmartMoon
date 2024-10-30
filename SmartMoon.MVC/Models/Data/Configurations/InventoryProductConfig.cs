using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.Data.Configurations
{
    public class InventoryProductConfig : IEntityTypeConfiguration<InventoryProduct>
    {
        public void Configure(EntityTypeBuilder<InventoryProduct> builder)
        {
            builder.HasKey(x => new { x.ProductId, x.InventoryId });
            builder.HasOne(x => x.Inventory).WithMany(x => x.inventoryProducts).HasForeignKey(x => x.InventoryId);
            builder.HasOne(x => x.Product).WithMany(x => x.inventoryProducts).HasForeignKey(x => x.ProductId);
        }
    }
}
