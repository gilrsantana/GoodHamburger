using Flunt.Validations;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Domain.Ordering.Entities;

public class Coupon : Entity
{
    public string Code { get; private set; } = null!;
    public decimal Value { get; private set; }
    public bool IsPercentage { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public int? UsageLimit { get; private set; }
    public int UsageCount { get; private set; }
    public decimal? MinimumOrderValue { get; private set; }
    public bool Active { get; private set; } = true;
    
    
    protected Coupon() { }
    
    private Coupon(string code, 
        decimal value, 
        bool isPercentage, 
        DateTime expirationDate, 
        int? usageLimit, 
        decimal? minimumOrderValue)
    {
        Code = code.ToUpper().Trim();
        Value = value;
        IsPercentage = isPercentage;
        ExpirationDate = expirationDate;
        UsageLimit = usageLimit;
        MinimumOrderValue = minimumOrderValue;
        UsageCount = 0;
        
        Validate();
    }

    public static Coupon Create(string code, 
        decimal value, 
        bool isPercentage, 
        DateTime expirationDate, 
        int? usageLimit, 
        decimal? minimumOrderValue)
    {
        var usage = usageLimit is < 0 ? null : usageLimit;
        var minimum = minimumOrderValue is < 0 ? null : minimumOrderValue;
        var coupon = new Coupon(code, value, isPercentage, expirationDate, usage, minimum);
        return coupon;
    }

    public Coupon UpdateCode(string code)
    {
        Code = code.ToUpper().Trim();
        Validate();
        return this;
    }

    public Coupon UpdateValue(int value)
    {
        Value = value;
        Validate();
        return this;
    }

    public Coupon UpdateIsPercentage(bool isPercentage)
    {
        IsPercentage = isPercentage;
        return this;
    }
    
    public Coupon UpdateExpirationDate(DateTime expirationDate)
    {
        ExpirationDate = expirationDate;
        Validate();
        return this;
    }
    
    public Coupon UpdateUsageLimit(int usageLimit)
    {
        UsageLimit = usageLimit is < 0 ? null : usageLimit; 
        Validate();
        return this;
    }
    
    public Coupon UpdateMinimumOrderValue(decimal? minimumOrderValue)
    {
        MinimumOrderValue = minimumOrderValue is < 0 ? null : minimumOrderValue;
        Validate();
        return this;
    }
    
    public Coupon UpdateActive(bool active)
    {
        Active = active;
        Validate();
        return this;   
    }
    
    public bool IsValidCoupon(decimal orderTotal)
    {
        if (!Active) 
            return false;
        if (DateTime.UtcNow > ExpirationDate) 
            return false;
        if (UsageLimit.HasValue && UsageCount >= UsageLimit.Value) 
            return false;
        if (orderTotal < (MinimumOrderValue ?? 0)) 
            return false;

        return true;
    }

    public void IncrementUsage()
    {
        if (UsageLimit.HasValue && UsageCount >= UsageLimit.Value)
        {
            AddNotification("Coupon.Usage", "Limite de uso do cupom atingido.");
            return;
        }
        UsageCount++;
    }
    
    public sealed override void Validate()
    {
        AddNotifications(new Contract<Coupon>()
            .Requires()
            .IsNotNullOrEmpty(Code, "Coupon.Code", "Código do cupom é obrigatório")
            .IsGreaterThan(Value, 0, "Coupon.Value", "Valor do cupom deve ser maior que 0.")
            .IsGreaterThan(ExpirationDate, DateTime.UtcNow, "Coupon.ExpirationDate", "Data de expiração deve ser no futuro.")
        );
        
        if (IsPercentage && Value > 100)
            AddNotification("Coupon.Value", "Percentual must be less than or equal to 100%.");
    }
}