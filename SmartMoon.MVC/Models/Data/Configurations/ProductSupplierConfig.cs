using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.Data.Configurations
{
    public class ProductSupplierConfig : IEntityTypeConfiguration<ProductSupplier>
    {
        public void Configure(EntityTypeBuilder<ProductSupplier> builder)
        {
            builder.HasKey(x => new { x.ProductId, x.SupplierId });
            builder.HasOne(x=>x.Supplier).WithMany(x=>x.productSuppliers).HasForeignKey(x=>x.SupplierId);
            builder.HasOne(x=>x.Product).WithMany(x=>x.productSuppliers).HasForeignKey(x=>x.ProductId);
        }
    }
}
