using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserDb> UserDb { get; set; }
        public DbSet<RequestDb> RequestDb { get; set; }
        public DbSet<ProductImageDb> ProductImageDb { get; set; }
        public DbSet<ProductDb> ProductDb { get; set; }
        public DbSet<OrderDb> OrderDb { get; set; } // Исправлено на PascalCase
        public DbSet<CategoryDb> CategoryDb { get; set; }
        public DbSet<CartItemDb> CartItems { get; set; }
        public DbSet<OrderItemDb> OrderItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурации для новых таблиц (CartItems и OrderItems)
            modelBuilder.Entity<CartItemDb>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId);

            modelBuilder.Entity<CartItemDb>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);

            modelBuilder.Entity<OrderItemDb>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItemDb>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            // Конфигурации для существующих таблиц
            modelBuilder.Entity<OrderDb>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<OrderDb>()
                .HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId);

            modelBuilder.Entity<ProductDb>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<ProductImageDb>()
                .HasOne(pi => pi.Product)
                .WithMany()
                .HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<RequestDb>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            // Конвертация DateTime для PostgreSQL
            // ИСПРАВЛЕННАЯ конвертация DateTime для PostgreSQL
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetColumnType("timestamp with time zone");
                        // ИЛИ используйте конвертер для приведения к локальному времени
                        property.SetValueConverter(
                            new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                                v => v.ToUniversalTime(),
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp with time zone");
                        property.SetValueConverter(
                            new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v));
                    }
                }
            }

        }
    }
}
