using GoodHamburger.Domain.Ordering.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Ordering;

public class OrderDiscountMap : IEntityTypeConfiguration<OrderDiscount>
{
    public void Configure(EntityTypeBuilder<OrderDiscount> builder)
    {
        builder.ToTable("ord_order_discounts");
        builder.HasKey(od => od.Id);
        builder.Ignore(od => od.Notifications);

        builder.Property(od => od.OrderId)
            .IsRequired()
            .HasColumnName("order_id");

        builder.Property(od => od.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(od => od.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("amount");

        builder.Property(od => od.CouponCode)
            .HasMaxLength(20)
            .HasColumnName("coupon_code");

        builder.Property(od => od.AppliedAt)
            .IsRequired()
            .HasColumnName("applied_at");

        builder.HasOne(od => od.Order)
            .WithMany(o => o.Discounts)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(od => od.OrderId)
            .HasDatabaseName("ix_ord_discount_order_id");

        builder.HasIndex(od => od.CouponCode)
            .HasDatabaseName("ix_ord_discount_coupon_code")
            .HasFilter("coupon_code IS NOT NULL");
    }
}