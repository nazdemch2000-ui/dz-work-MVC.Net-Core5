using Microsoft.EntityFrameworkCore;
using MVC_Dz5.Models;

namespace MVC_Dz5.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<BookImage> BookImages => Set<BookImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book 1 — ∞ Comment, с каскадным удалением
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Comments)
                .WithOne(c => c.Book)
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book 1 — ∞ BookImage, с каскадным удалением
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Images)
                .WithOne(i => i.Book)
                .HasForeignKey(i => i.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Точность для денежного поля
            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
