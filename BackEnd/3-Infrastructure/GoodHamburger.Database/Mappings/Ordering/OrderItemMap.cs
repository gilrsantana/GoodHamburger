using GoodHamburger.Domain.Ordering.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Ordering;

public class OrderItemMap : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("ord_order_items");
        builder.HasKey(oi => oi.Id);
        builder.Ignore(oi => oi.Notifications);
        
        builder.Property(oi => oi.OrderId)
            .IsRequired()
            .HasColumnName("order_id");
        builder.Property(oi => oi.MenuItemId)
            .IsRequired()
            .HasColumnName("menu_item_id");
        
        builder.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("product_name");

        builder.Property(oi => oi.Sku)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("sku");

        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("unit_price");

        builder.Property(oi => oi.Quantity)
            .IsRequired()
            .HasColumnName("quantity");

        builder.Property(oi => oi.Note)
            .HasMaxLength(255)
            .HasColumnName("note");
        
        builder.OwnsMany(oi => oi.Details, d =>
        {
            d.ToTable("ord_order_item_details");
            d.WithOwner().HasForeignKey(x => x.OrderItemId);
            d.HasKey(x => x.Id);
            d.Ignore(x => x.Notifications);

            d.Property(x => x.IngredientId)
                .IsRequired()
                .HasColumnName("ingredient_id");

            d.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");

            d.Property(x => x.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("price");

            d.Property(x => x.IsRemoved)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("is_removed");

            d.HasIndex(x => x.IsRemoved)
                .HasDatabaseName("ix_ord_detail_is_removed");
        });

        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.MenuItemId);
    }
}