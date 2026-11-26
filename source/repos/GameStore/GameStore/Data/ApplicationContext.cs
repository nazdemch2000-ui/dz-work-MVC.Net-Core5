using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> context) : base(context)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().Property(p => p.PurchasePrice).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.RetailPrice).HasPrecision(18, 2);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", Description = "Экшн-игры" },
                new Category { Id = 2, Name = "RPG", Description = "Ролевые игры" },
                new Category { Id = 3, Name = "Strategy", Description = "Стратегии" },
                new Category { Id = 4, Name = "Sports", Description = "Спортивные игры" },
                new Category { Id = 5, Name = "Adventure", Description = "Приключения" }
            );
        }
    }
}
