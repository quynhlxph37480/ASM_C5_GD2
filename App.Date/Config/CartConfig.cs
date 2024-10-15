using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using App.Data.Entities;

namespace OnTap_NET104.Config
{
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {

            builder.HasKey(x => x.Username);
			builder.HasOne(p => p.Account).WithOne(p => p.Cart).HasForeignKey<Account>(p => p.Username).OnDelete(DeleteBehavior.Cascade);
		}
    }
}
