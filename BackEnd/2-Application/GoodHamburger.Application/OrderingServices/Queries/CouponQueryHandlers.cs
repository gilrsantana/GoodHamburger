using GoodHamburger.Domain.Ordering.Entities;
using GoodHamburger.Domain.Repositories.Ordering;
using GoodHamburger.Shared.Handlers;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.OrderingServices.Queries;

public interface IGetAllCouponsHandler
{
    Task<Result<GetAllCouponsResponse>> HandleAsync(GetAllCouponsQuery query, CancellationToken cancellationToken = default);
}

public interface IGetCouponByIdHandler
{
    Task<Result<GetCouponByIdResponse>> HandleAsync(GetCouponByIdQuery query, CancellationToken cancellationToken = default);
}

public interface IGetCouponByCodeHandler
{
    Task<Result<GetCouponByCodeResponse>> HandleAsync(GetCouponByCodeQuery query, CancellationToken cancellationToken = default);
}

public interface IGetActiveCouponsHandler
{
    Task<Result<GetActiveCouponsResponse>> HandleAsync(GetActiveCouponsQuery query, CancellationToken cancellationToken = default);
}

public class CouponQueryHandlers : IGetAllCouponsHandler, IGetCouponByIdHandler, IGetCouponByCodeHandler, IGetActiveCouponsHandler
{
    private readonly ICouponRepository _couponRepository;
    private readonly ILogger<CouponQueryHandlers> _logger;

    public CouponQueryHandlers(ICouponRepository couponRepository, ILogger<CouponQueryHandlers> logger)
    {
        _couponRepository = couponRepository;
        _logger = logger;
    }

    public async Task<Result<GetAllCouponsResponse>> HandleAsync(GetAllCouponsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var coupons = await _couponRepository.GetAllAsync(cancellationToken);
            var totalCount = coupons.Count;
            var items = coupons
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            return Result<GetAllCouponsResponse>.Success(new GetAllCouponsResponse(items, totalCount, query.Page, query.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying coupons");
            return Result<GetAllCouponsResponse>.Failure(new Error("Coupon.QueryError", "Error retrieving coupons."));
        }
    }

    public async Task<Result<GetCouponByIdResponse>> HandleAsync(GetCouponByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var coupon = await _couponRepository.GetByIdAsync(query.Id, cancellationToken);
            return Result<GetCouponByIdResponse>.Success(new GetCouponByIdResponse(coupon != null ? MapToDto(coupon) : null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying coupon {CouponId}", query.Id);
            return Result<GetCouponByIdResponse>.Failure(new Error("Coupon.QueryError", "Error retrieving coupon."));
        }
    }

    public async Task<Result<GetCouponByCodeResponse>> HandleAsync(GetCouponByCodeQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var coupon = await _couponRepository.GetByCodeAsync(query.Code, cancellationToken);
            return Result<GetCouponByCodeResponse>.Success(new GetCouponByCodeResponse(coupon != null ? MapToDto(coupon) : null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying coupon code {Code}", query.Code);
            return Result<GetCouponByCodeResponse>.Failure(new Error("Coupon.QueryError", "Error retrieving coupon."));
        }
    }

    public async Task<Result<GetActiveCouponsResponse>> HandleAsync(GetActiveCouponsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var coupons = await _couponRepository.GetActiveCouponsAsync(cancellationToken);
            var totalCount = coupons.Count;
            var items = coupons
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            return Result<GetActiveCouponsResponse>.Success(new GetActiveCouponsResponse(items, totalCount, query.Page, query.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying active coupons");
            return Result<GetActiveCouponsResponse>.Failure(new Error("Coupon.QueryError", "Error retrieving active coupons."));
        }
    }

    private static CouponDto MapToDto(Coupon coupon)
    {
        return new CouponDto(
            coupon.Id,
            coupon.Code,
            coupon.Value,
            coupon.IsPercentage,
            coupon.ExpirationDate,
            coupon.UsageLimit,
            coupon.UsageCount,
            coupon.MinimumOrderValue,
            coupon.Active
        );
    }
}
