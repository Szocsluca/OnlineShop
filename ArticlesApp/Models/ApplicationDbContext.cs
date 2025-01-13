using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineShopApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ProductCart> ProductCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity<ProductCart>()
            .HasKey(ab => new {
                ab.Id,
                ab.ProductId,
                ab.CartId
            });
   

            modelBuilder.Entity<ProductCart>()
            .HasOne(ab => ab.Product)
            .WithMany(ab => ab.ProductCarts)
            .HasForeignKey(ab => ab.ProductId);
            modelBuilder.Entity<ProductCart>()
            .HasOne(ab => ab.Cart)
            .WithMany(ab => ab.ProductCarts)
            .HasForeignKey(ab => ab.CartId);

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.ProductId, r.UserId })
                .IsUnique();
        }
    }
}
