using BaseLibrary.Entities;
using BaseLibrary.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }


        /*public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && e.State is EntityState.Added or EntityState.Modified);

            foreach (var entry in entries)
            {
                ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }*/


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureProduct(modelBuilder);
            ConfigureCart(modelBuilder);
            ConfigureOrder(modelBuilder);
            ConfigureReview(modelBuilder);
            ConfigurePayment(modelBuilder);

            SeedData(modelBuilder);
        }

        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }

        private static void ConfigureProduct(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Discount)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureCart(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique();
        }

        private static void ConfigureOrder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("numeric(18,2)");
        }

        private static void ConfigureReview(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigurePayment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne()
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("numeric(18,2)");
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Фиксированные BCrypt-хэши для тестовых пользователей
            const string passwordHash1 = "$2a$11$KlmfsQMS9aHuU56jghzUCeRj3Y5L8M07j6apT14Tlh27QXr6wFi3K"; // для «q1w2e3123»
            const string passwordHash2 = "$2a$11$UFGyAKF2jCbnBtaGgz9mVOX4Fev1WABX6r7PVZJ3oZDWxdtPqXkWy"; // другой хэш

            // Статическое время, не меняющееся при каждой сборке
            var fixedCreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Баходурхон",
                    LastName = "Турсунов",
                    Username = "bakha",
                    Email = "tursunovb18@gmail.com",
                    PasswordHash = passwordHash1,
                    Role = "Admin",
                    CreatedAt = fixedCreatedAt,
                    UpdatedAt = fixedCreatedAt
                },
                new User
                {
                    Id = 2,
                    FirstName = "Иван",
                    LastName = "Петров",
                    Username = "vanya01",
                    Email = "ivan.petrov@example.com",
                    PasswordHash = passwordHash2,
                    Role = "User",
                    CreatedAt = fixedCreatedAt,
                    UpdatedAt = fixedCreatedAt
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Электроника", CreatedAt = fixedCreatedAt, UpdatedAt = fixedCreatedAt },
                new Category { Id = 2, Name = "Одежда", CreatedAt = fixedCreatedAt, UpdatedAt = fixedCreatedAt }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Смартфон",
                    Description = "Современный смартфон с AMOLED-экраном",
                    Price = 599.99m,
                    Stock = 50,
                    Discount = 0m,
                    CategoryId = 1,
                    CreatedAt = fixedCreatedAt,
                    UpdatedAt = fixedCreatedAt
                },
                new Product
                {
                    Id = 2,
                    Name = "Футболка",
                    Description = "100% хлопок, белого цвета",
                    Price = 19.99m,
                    Stock = 200,
                    Discount = 5m,
                    CategoryId = 2,
                    CreatedAt = fixedCreatedAt,
                    UpdatedAt = fixedCreatedAt
                }
            );

            modelBuilder.Entity<Cart>().HasData(
                new Cart { Id = 1, UserId = 2, CreatedAt = fixedCreatedAt, UpdatedAt = fixedCreatedAt }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    UserId = 2,
                    TotalAmount = 619.98m,
                    Status = Statuses.Pending,
                    CreatedAt = fixedCreatedAt,
                    UpdatedAt = fixedCreatedAt
                }
            );

            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 599.99m, CreatedAt = fixedCreatedAt, UpdatedAt = fixedCreatedAt },
                new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 1, UnitPrice = 19.99m, CreatedAt = fixedCreatedAt, UpdatedAt = fixedCreatedAt }
            );

            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    Id = 1,
                    UserId = 2,
                    ProductId = 1,
                    Rating = 5,
                    Comment = "Отличный смартфон, рекомендую!",
                    CreatedAt = fixedCreatedAt,
                    UpdatedAt = fixedCreatedAt
                }
            );
        }


    }
}
