using GoodHamburger.Domain.Ordering.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Sales;

public class CouponMap : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("sales_coupons");
        builder.HasKey(c => c.Id);

        builder.Ignore(c => c.Notifications);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("code");

        builder.Property(c => c.Value)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("discount_value");

        builder.Property(c => c.IsPercentage)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_percentage");

        builder.Property(c => c.ExpirationDate)
            .IsRequired()
            .HasColumnName("expiration_date");

        builder.Property(c => c.UsageLimit)
            .HasColumnName("usage_limit");

        builder.Property(c => c.UsageCount)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("usage_count");

        builder.Property(c => c.MinimumOrderValue)
            .HasColumnType("decimal(18,2)")
            .HasColumnName("min_order_value");

        builder.Property(c => c.Active)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasDatabaseName("ix_sal_coupon_code");

        builder.HasIndex(c => c.Active)
            .HasDatabaseName("ix_sal_coupon_active");
    }
}