using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using App.Data.Entities;

namespace OnTap_NET104.Config
{
    public class Account_Config : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            //Xet khoa chinh
            builder.HasKey(x => x.Username);
        }
    }
}
