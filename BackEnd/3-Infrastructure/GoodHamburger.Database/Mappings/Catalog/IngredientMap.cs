using GoodHamburger.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Catalog;

public class IngredientMap : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("cat_ingredients");
        builder.HasKey(i => i.Id);

        builder.Ignore(i => i.Notifications);
        
        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(i => i.Active)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");
        
        builder.Property(i => i.SalePrice)
            .HasColumnType("decimal(18,2)")
            .HasColumnName("sale_price");

        builder.Property(i => i.ReferenceCostPrice)
            .IsRequired()
            .HasColumnType("decimal(18,4)")
            .HasDefaultValue(0)
            .HasColumnName("reference_cost_price");
        
        builder.HasIndex(i => i.Name)
            .HasDatabaseName("ix_cat_ingredient_name");
            
        builder.HasIndex(i => i.Active)
            .HasDatabaseName("ix_cat_ingredient_active");
    }
}