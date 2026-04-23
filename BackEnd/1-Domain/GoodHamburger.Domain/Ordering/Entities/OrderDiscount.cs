using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Domain.Ordering.Entities;

public class OrderDiscount : Entity
{
    public string Name { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public string? CouponCode { get; private set; }
    public DateTime AppliedAt { get; private set; }
    
    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }
    
    protected OrderDiscount() { }
    
    public OrderDiscount(Guid orderId, string name, decimal amount, string? couponCode = null)
    {
        OrderId = orderId;
        Name = name;
        Amount = amount;
        CouponCode = couponCode;
        AppliedAt = DateTime.UtcNow;
        
        Validate();
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<OrderDiscount>()
            .Requires()
            .IsGreaterOrEqualsThan(Amount, 0, "OrderDiscount.Amount", "O valor do desconto não pode ser negativo.")
        );
    }
}