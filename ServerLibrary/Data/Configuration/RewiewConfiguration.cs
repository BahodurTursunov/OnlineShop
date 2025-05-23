using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ServerLibrary.Data.Configuration
{
    internal class RewiewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Rating)
                .IsRequired();

            builder.Property(e => e.Comment)
                .HasMaxLength(1000);

            builder.HasOne(p => p.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(p => p.ProductId);

            builder
             .HasOne(r => r.User)
             .WithMany()
             .HasForeignKey(r => r.UserId);
        }
    }
}
