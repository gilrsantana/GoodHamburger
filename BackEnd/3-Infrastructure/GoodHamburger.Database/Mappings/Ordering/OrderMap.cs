using GoodHamburger.Domain.Ordering.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Ordering;

public class OrderMap : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("ord_orders");
        builder.HasKey(o => o.Id);
        builder.Ignore(o => o.Notifications);

        builder.Property(o => o.CustomerId)
            .IsRequired()
            .HasColumnName("customer_id");

        builder.Property(o => o.OrderDate)
            .IsRequired()
            .HasColumnName("order_date");

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("status");

        builder.Property(o => o.CancelReason)
            .HasMaxLength(255)
            .HasColumnName("cancel_reason");

        builder.Property(o => o.DeliveryFee)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("delivery_fee");

        builder.OwnsOne(o => o.DeliveryAddress, addr =>
        {
            addr.Ignore(a => a.Notifications);
            addr.Ignore(a => a.StreetType);
            addr.Ignore(a => a.Neighborhood);
            
            addr.Property(a => a.StreetTypeId)
                .HasColumnName("delivery_street_type_id");
            
            addr.Property(a => a.StreetName)
                .HasMaxLength(150)
                .HasColumnName("delivery_street_name");
            
            addr.Property(a => a.Number)
                .HasMaxLength(20)
                .HasColumnName("delivery_number");
            
            addr.Property(a => a.ZipCode)
                .HasColumnType("char(8)")
                .HasColumnName("delivery_zip_code");
            
            addr.Property(a => a.NeighborhoodId)
                .HasColumnName("delivery_neighborhood_id");
            
            addr.Property(a => a.Complement)
                .HasMaxLength(100)
                .HasColumnName("delivery_complement");
            
            addr.Ignore(a => a.StreetType);
            addr.Ignore(a => a.Neighborhood);
        });
        
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(o => o.Discounts)
            .WithOne(od => od.Order)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(o => o.Coupons)
            .WithOne(oc => oc.Order)
            .HasForeignKey(oc => oc.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(o => o.CustomerId).HasDatabaseName("ix_ord_order_customer");
        builder.HasIndex(o => o.Status).HasDatabaseName("ix_ord_order_status");
        builder.HasIndex(o => o.OrderDate).HasDatabaseName("ix_ord_order_date");
    }
}