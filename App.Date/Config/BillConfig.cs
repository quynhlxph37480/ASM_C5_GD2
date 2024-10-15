using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using App.Data.Entities;

namespace OnTap_NET104.Config
{
    public class BillConfig : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(p => p.Account).WithMany(x => x.Bills).HasForeignKey(x => x.Username);
        }
    }
}
