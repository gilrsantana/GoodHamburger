using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Domain.Ordering.Entities;

public class OrderCoupon : Entity
{
    public Guid OrderId { get; private set; }
    public Guid CouponId { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public DateTime AppliedAt { get; private set; }
    public Order? Order { get; private set; }
    public Coupon? Coupon { get; private set; }

    protected OrderCoupon() { }

    public OrderCoupon(Guid orderId, Guid couponId, decimal discountAmount)
    {
        OrderId = orderId;
        CouponId = couponId;
        DiscountAmount = discountAmount;
        AppliedAt = DateTime.UtcNow;
        
        Validate();
    }

    public static OrderCoupon Create(Guid orderId, Guid couponId, decimal discountAmount)
    {
        return new OrderCoupon(orderId, couponId, discountAmount);
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<OrderCoupon>()
            .Requires()
            .AreNotEquals(OrderId, Guid.Empty, "OrderCoupon.OrderId", "ID do pedido é obrigatório.")
            .AreNotEquals(CouponId, Guid.Empty, "OrderCoupon.CouponId", "ID do cupom é obrigatório.")
            .IsGreaterThan(DiscountAmount, 0, "OrderCoupon.DiscountAmount", "Desconto deve ser maior que zero.")
        );
    }
}
