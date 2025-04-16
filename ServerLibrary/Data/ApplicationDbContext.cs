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
        public DbSet<Review> Rewiews { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 1-N: User -> Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("numeric(18,2)");

            // 1-N: User -> Cart
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-N: Order -> OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.UnitPrice)
                .HasColumnType("numeric(18,2)");

            // 1-N: Product -> OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Нельзя удалить продукт, если есть заказы

            // 1-N: Cart -> CartItems
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-N: Product -> CartItems
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Нельзя удалить продукт, если он в корзине

            // 1-N: Product -> Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-N: User -> Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-N: Payment -> Order
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne()
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .Property(o => o.Amount)
                .HasColumnType("numeric(18,2)");

            // 1-N: Category -> Products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .Property(o => o.Discount)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<Product>()
                .Property(o => o.Price)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();  // Убедись, что для Id включена генерация значений


            // 1-N: Category -> ChildCategory
            modelBuilder.Entity<ChildCategory>()
                .HasOne(cc => cc.Category)
                .WithMany()
                .HasForeignKey(cc => cc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);



            // Добавление пользователей
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique(); // Устанавливаем уникальность имени пользователя

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Баходурхон",
                    LastName = "Турсунов",
                    Username = "bakha",
                    Email = "tursunovb18@gmail.com",
                    PasswordHash = "q1w2e3123", // TODO: заменить на хэшированный пароль
                    RoleId = 1,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    FirstName = "Иван",
                    LastName = "Петров",
                    Username = "vanya01",
                    Email = "ivan.petrov@example.com",
                    PasswordHash = "q1w2e3321",
                    RoleId = 0,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Добавление категорий
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Электроника", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 2, Name = "Одежда", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            // Добавление товаров
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
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
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
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Добавление корзин (пустые)
            modelBuilder.Entity<Cart>().HasData(
                new Cart { Id = 1, UserId = 2 }
            );

            // Добавление заказов (примерный шаблон)
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    UserId = 2,
                    TotalAmount = 619.98m,
                    Status = Statuses.Pending,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Добавление элементов заказа
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    UnitPrice = 599.99m
                },
                new OrderItem
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 2,
                    Quantity = 1,
                    UnitPrice = 19.99m
                }
            );

            // Добавление отзывов
            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    Id = 1,
                    UserId = 2,
                    ProductId = 1,
                    Rating = 5,
                    Comment = "Отличный смартфон, рекомендую!"
                }
            );


        }

    }
}
