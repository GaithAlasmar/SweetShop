using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SweetShop.Models;

namespace SweetShop.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{

    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = default!;
    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = default!;
    public DbSet<SiteSettings> SiteSettings { get; set; } = default!;
    public DbSet<Coupon> Coupons { get; set; } = default!;
    public DbSet<ProductVariant> ProductVariants { get; set; } = default!;
    public DbSet<ProductReview> ProductReviews { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Global Query Filters for Soft Delete
        builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);

        // Make relationships optional to silence query filter warnings
        builder.Entity<OrderDetail>().HasOne(od => od.Order).WithMany(o => o.OrderDetails).HasForeignKey(od => od.OrderId).IsRequired(false);
        builder.Entity<ProductReview>().HasOne(pr => pr.Product).WithMany(p => p.Reviews).HasForeignKey(pr => pr.ProductId).IsRequired(false);
        builder.Entity<ProductVariant>().HasOne(pv => pv.Product).WithMany(p => p.Configurations).HasForeignKey(pv => pv.ProductId).IsRequired(false);
        builder.Entity<ShoppingCartItem>().HasOne(sci => sci.Product).WithMany().HasForeignKey(sci => sci.ProductId).IsRequired(false);
    }
}

