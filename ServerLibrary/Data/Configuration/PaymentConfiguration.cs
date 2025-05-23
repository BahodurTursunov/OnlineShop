using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ServerLibrary.Data.Configuration
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)

        {
            builder.HasKey(x => x.Id);

            builder
             .HasOne(p => p.Order)
             .WithOne(o => o.Payment)
             .HasForeignKey<Payment>(p => p.OrderId)
             .OnDelete(DeleteBehavior.Restrict);

            builder
                .Property(p => p.Amount)
                .HasColumnType("numeric(18,2)");

            builder.Property(x => x.PaymentMethod)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30);
        }
    }
}
