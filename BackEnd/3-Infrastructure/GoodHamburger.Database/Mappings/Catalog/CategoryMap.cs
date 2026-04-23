using GoodHamburger.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Catalog;

public class CategoryMap : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("cat_categories");
        builder.HasKey(c => c.Id);

        builder.Ignore(c => c.Notifications);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("description");

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("slug");

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnName("category_type");

        builder.Property(c => c.Active)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(c => c.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("display_order");

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(255)
            .HasColumnName("image_url");
        
        builder.HasMany(c => c.Items)
            .WithOne(m => m.Category)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(c => c.Slug)
            .IsUnique()
            .HasDatabaseName("ix_cat_category_slug");

        builder.HasIndex(c => c.DisplayOrder)
            .HasDatabaseName("ix_cat_category_display_order");
    }
}