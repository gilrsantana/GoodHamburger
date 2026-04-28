namespace GoodHamburger.FrontEnd.Clients.Coupons;

public class CouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public double Value { get; set; }
    public bool IsPercentage { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public double? MinimumOrderValue { get; set; }
    public bool Active { get; set; }
}

public class CreateCouponCommand { public string Code { get; set; } = string.Empty; public double Value { get; set; } public bool IsPercentage { get; set; } public DateTimeOffset ExpirationDate { get; set; } public int? UsageLimit { get; set; } public double? MinimumOrderValue { get; set; } }
public record CreateCouponResponse(Guid Id, string Code, bool Succeeded, IEnumerable<string>? Errors);
public record GetAllCouponsResponse(IEnumerable<CouponDto> Items, int TotalCount, int Page, int PageSize);
public record GetCouponByIdResponse(CouponDto? Coupon);
public record GetCouponByCodeResponse(CouponDto? Coupon);
public record GetActiveCouponsResponse(IEnumerable<CouponDto> Items, int TotalCount, int Page, int PageSize);
public class UpdateCouponCommand { public Guid CouponId { get; set; } public string? Code { get; set; } public double? Value { get; set; } public bool? IsPercentage { get; set; } public DateTimeOffset? ExpirationDate { get; set; } public int? UsageLimit { get; set; } public double? MinimumOrderValue { get; set; } public bool? Active { get; set; } }
public record UpdateCouponResponse(Guid Id, string Code, bool Succeeded, IEnumerable<string>? Errors);
public record CancelCouponResponse(Guid Id, bool Succeeded, IEnumerable<string>? Errors);
