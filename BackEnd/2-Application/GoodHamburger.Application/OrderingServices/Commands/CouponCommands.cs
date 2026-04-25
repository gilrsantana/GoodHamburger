namespace GoodHamburger.Application.OrderingServices.Commands;

public record CreateCouponCommand(
    string Code,
    decimal Value,
    bool IsPercentage,
    DateTime ExpirationDate,
    int? UsageLimit = null,
    decimal? MinimumOrderValue = null
);

public record CreateCouponResponse(
    Guid Id,
    string Code,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateCouponCommand(
    Guid CouponId,
    string? Code = null,
    decimal? Value = null,
    bool? IsPercentage = null,
    DateTime? ExpirationDate = null,
    int? UsageLimit = null,
    decimal? MinimumOrderValue = null,
    bool? Active = null
);

public record UpdateCouponResponse(
    Guid Id,
    string Code,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record CancelCouponCommand(Guid CouponId);

public record CancelCouponResponse(
    Guid Id,
    bool Succeeded,
    IEnumerable<string> Errors
);
