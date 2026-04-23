using GoodHamburger.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Catalog;

public class MenuItemMap : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("cat_menu_items");
        builder.HasKey(m => m.Id);
        builder.Ignore(m => m.Notifications);
        
        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("name");

        builder.Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(m => m.Slug)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("slug");

        builder.Property(m => m.Sku)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("sku");

        builder.Property(m => m.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("price");

        builder.Property(m => m.ImageUrl)
            .HasMaxLength(255)
            .HasColumnName("image_url");

        builder.Property(m => m.Active)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(m => m.Calories)
            .HasColumnName("calories");

        builder.Property(m => m.IsAvailable)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_available");

        // Índices
        builder.HasIndex(m => m.Slug).IsUnique().HasDatabaseName("ix_cat_menu_item_slug");
        builder.HasIndex(m => m.Sku).IsUnique().HasDatabaseName("ix_cat_menu_item_sku");
        builder.HasIndex(m => m.CategoryId).HasDatabaseName("ix_cat_menu_item_category");
    }
}