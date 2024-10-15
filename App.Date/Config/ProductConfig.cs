using Microsoft.EntityFrameworkCore;
using App.Data.Entities;

namespace OnTap_NET104.Config
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");

            builder.HasMany(x => x.CartDetails).WithOne(x => x.Product).HasForeignKey(x => x.ProductId);

            builder.HasMany(x => x.BillDetails).WithOne(x => x.Product).HasForeignKey(x => x.ProductId);
        }
    }
}
