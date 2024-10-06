using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.Data.Configurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Price).HasDefaultValue(0.0m);
            builder.Property(x => x.Quantity).HasDefaultValue(0);
            
        }
    }
}
