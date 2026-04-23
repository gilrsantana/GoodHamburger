using GoodHamburger.Domain.Accounts.Entities;
using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Ordering.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GoodHamburger.Database.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    // Accounts
    public DbSet<CustomerProfile> Customers => Set<CustomerProfile>();
    public DbSet<EmployeeProfile> Employees => Set<EmployeeProfile>();

    // Catalog
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuItemIngredient> MenuItemIngredients => Set<MenuItemIngredient>();

    // Ordering
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderDiscount> OrderDiscounts => Set<OrderDiscount>();
    public DbSet<OrderItemDetail> OrderItemDetails => Set<OrderItemDetail>();
    public DbSet<OrderCoupon> OrderCoupons => Set<OrderCoupon>();
    

    // Sales
    public DbSet<Coupon> Coupons => Set<Coupon>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
                
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}