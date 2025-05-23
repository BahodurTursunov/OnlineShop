using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ServerLibrary.Data.Configuration
{
    internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                 .HasOne(oi => oi.Order)
                 .WithMany(o => o.OrderItems)
                 .HasForeignKey(oi => oi.OrderId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // если ещё не добавлено, добавить

            builder
                .Property(oi => oi.UnitPrice)
                .HasColumnType("numeric(18,2)");
        }
    }
}
