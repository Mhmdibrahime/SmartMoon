using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMoon.MVC.Models.Entities;

namespace SmartMoon.MVC.Models.Data.Configurations
{
    public class ExpenseConfig : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.MoneyDrawer).WithMany(x=>x.Expenses).HasForeignKey(x=>x.MoneyDrawerId);
        }
    }
}
