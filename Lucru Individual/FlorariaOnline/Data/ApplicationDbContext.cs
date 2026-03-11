using FlorariaOnline.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlorariaOnline.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Flower> Flowers => Set<Flower>();
    public DbSet<BouquetProduct> BouquetProducts => Set<BouquetProduct>();
    public DbSet<BouquetProductItem> BouquetProductItems => Set<BouquetProductItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<CustomBouquet> CustomBouquets => Set<CustomBouquet>();
    public DbSet<CustomBouquetItem> CustomBouquetItems => Set<CustomBouquetItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<BouquetProductItem>()
            .HasOne(x => x.BouquetProduct)
            .WithMany(p => p.Items)
            .HasForeignKey(x => x.BouquetProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CustomBouquet>()
            .HasOne(cb => cb.OrderItem)
            .WithOne(oi => oi.CustomBouquet)
            .HasForeignKey<CustomBouquet>(cb => cb.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CustomBouquetItem>()
            .HasOne(x => x.CustomBouquet)
            .WithMany(cb => cb.Items)
            .HasForeignKey(x => x.CustomBouquetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}