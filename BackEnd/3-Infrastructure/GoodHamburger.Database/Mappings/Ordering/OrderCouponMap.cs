using GoodHamburger.Domain.Ordering.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Database.Mappings.Ordering;

public class OrderCouponMap : IEntityTypeConfiguration<OrderCoupon>
{
    public void Configure(EntityTypeBuilder<OrderCoupon> builder)
    {
        builder.Ignore(oc => oc.Notifications);
    }
}