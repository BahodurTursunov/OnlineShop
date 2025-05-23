using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ServerLibrary.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder
                .Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // не каскад, чтобы сохранить историю


            builder
                .Property(o => o.TotalAmount)
                .HasColumnType("numeric(18,2)");

            builder
                .Property(o => o.OrderDate)
                .IsRequired(); // если у тебя есть такое поле

            builder
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            builder
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId);
        }

    }
}
