namespace GoodHamburger.Application.OrderingServices.Queries;

public record GetAllCouponsQuery(int Page = 1, int PageSize = 10);

public record GetAllCouponsResponse(
    IReadOnlyList<CouponDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetCouponByIdQuery(Guid Id);

public record GetCouponByIdResponse(CouponDto? Coupon);

public record GetCouponByCodeQuery(string Code);

public record GetCouponByCodeResponse(CouponDto? Coupon);

public record GetActiveCouponsQuery(int Page = 1, int PageSize = 10);

public record GetActiveCouponsResponse(
    IReadOnlyList<CouponDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record CouponDto(
    Guid Id,
    string Code,
    decimal Value,
    bool IsPercentage,
    DateTime ExpirationDate,
    int? UsageLimit,
    int UsageCount,
    decimal? MinimumOrderValue,
    bool Active
);
