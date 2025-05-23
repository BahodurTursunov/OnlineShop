using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ServerLibrary.Data.Configuration
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.HasKey(x => x.Id);


            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            builder.Property(x => x.isActive)
                .HasDefaultValue(true);

        }
    }
}
