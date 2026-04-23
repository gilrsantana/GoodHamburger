using GoodHamburger.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Catalog;

public class MenuItemIngredientMap : IEntityTypeConfiguration<MenuItemIngredient>
{
    public void Configure(EntityTypeBuilder<MenuItemIngredient> builder)
    {
        builder.ToTable("cat_menu_item_ingredients");
        builder.HasKey(x => new { x.MenuItemId, x.IngredientId });
        builder.Ignore(x => x.Notifications);

        builder.Property(x => x.IngredientId)
            .HasColumnName("IngredientId");

        builder.Property(x => x.IsRemovable)
            .IsRequired()
            .HasColumnName("is_removable");

        builder.HasOne(x => x.Ingredient)
            .WithMany()
            .HasForeignKey(x => x.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
