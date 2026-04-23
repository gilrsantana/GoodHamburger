using Flunt.Validations;
using GoodHamburger.Domain.Ordering.Enums;
using GoodHamburger.Shared.Entities.Base;
using GoodHamburger.Shared.ValueObjects.Locations;

namespace GoodHamburger.Domain.Ordering.Entities;

public class Order : Entity
{
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? CancelReason { get; private set; }
    public Address DeliveryAddress { get; private set; } = null!; 
    public decimal DeliveryFee { get; private set; }
    
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items;
    
    private readonly List<OrderDiscount> _discounts = new();
    public IReadOnlyCollection<OrderDiscount> Discounts => _discounts;

    private readonly List<OrderCoupon> _coupons = new();
    public IReadOnlyCollection<OrderCoupon> Coupons => _coupons;

    protected Order() { }
    
    public Order(Guid customerId, Address deliveryAddress, decimal deliveryFee)
    {
        CustomerId = customerId;
        DeliveryAddress = deliveryAddress;
        DeliveryFee = deliveryFee;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        
        AddNotifications(new Contract<Order>()
            .Requires()
            .AreNotEquals(CustomerId, Guid.Empty, "Order.CustomerId", "Client is required")
            .IsNotNull(DeliveryAddress, "Order.DeliveryAddress", "Address is required.")
            .IsGreaterThan(DeliveryFee, -1, "Order.DeliveryFee", "Delivery fee must be greater than 0.")
        );
    }

    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Não é possível adicionar itens a um pedido já processado.");

        _items.Add(item);
    }
    
    public void ApplyDiscount(OrderDiscount discount)
    {
        // Regra: O desconto não pode tornar o pedido negativo
        if (TotalDiscounts + discount.Amount > TotalItemsPrice)
        {
            AddNotification("Order.Discount", "O valor do desconto excede o total dos itens.");
            return;
        }

        _discounts.Add(discount);
    }

    public void ApplyCategoryDiscount()
    {
        // var discountService = new CategoryDiscountService();
        // var discount = discountService.CalculateBestDiscount(this);
        //
        // if (discount != null)
        // {
        //     ApplyDiscount(discount);
        // }
    }

    public void ApplyCoupon(Coupon coupon)
    {
        if (Status != OrderStatus.Pending)
        {
            AddNotification("Order.Coupon", "Não é possível aplicar cupom a um pedido já processado.");
            return;
        }

        if (!coupon.IsValidCoupon(TotalItemsPrice))
        {
            AddNotification("Order.Coupon", "Cupom inválido ou não aplicável a este pedido.");
            return;
        }

        var discountAmount = coupon.IsPercentage 
            ? TotalItemsPrice * (coupon.Value / 100) 
            : coupon.Value;
        
        if (_coupons.Any(oc => oc.CouponId == coupon.Id))
        {
            AddNotification("Order.Coupon", "Este cupom já foi aplicado a este pedido.");
            return;
        }

        var orderCoupon = OrderCoupon.Create(Id, coupon.Id, discountAmount);
        _coupons.Add(orderCoupon);
        
        coupon.IncrementUsage();
        
        var discount = new OrderDiscount(Id, $"Coupon: {coupon.Code}", discountAmount, coupon.Code);
        ApplyDiscount(discount);
    }

    
    public decimal TotalItemsPrice => _items.Sum(x => x.TotalItemPrice);
    public decimal TotalDiscounts => _discounts.Sum(x => x.Amount);
    public decimal FinalAmount => (TotalItemsPrice - TotalDiscounts) + DeliveryFee;

    // Mudanças de Estado (Workflow)
    public void ConfirmPayment() => Status = OrderStatus.Confirmed;
    public void StartPreparation() => Status = OrderStatus.InPreparation;
    public void SetAsReady() => Status = OrderStatus.Ready;
    public void Dispatch() => Status = OrderStatus.InDelivery;
    public void Complete() => Status = OrderStatus.Completed;
    public void Cancel(string reason) 
    {
        CancelReason = reason;
        Status = OrderStatus.Cancelled;
    }

    public override void Validate()
    {
        throw new NotImplementedException();
    }
}