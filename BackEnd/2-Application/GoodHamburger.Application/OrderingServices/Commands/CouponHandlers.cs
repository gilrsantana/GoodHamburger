using GoodHamburger.Domain.Ordering.Entities;
using GoodHamburger.Domain.Repositories.Ordering;
using GoodHamburger.Shared.Handlers;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.OrderingServices.Commands;

public interface ICreateCouponHandler
{
    Task<Result<CreateCouponResponse>> HandleAsync(CreateCouponCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateCouponHandler
{
    Task<Result<UpdateCouponResponse>> HandleAsync(UpdateCouponCommand command, CancellationToken cancellationToken = default);
}

public interface ICancelCouponHandler
{
    Task<Result<CancelCouponResponse>> HandleAsync(CancelCouponCommand command, CancellationToken cancellationToken = default);
}

public class CouponHandlers : ICreateCouponHandler, IUpdateCouponHandler, ICancelCouponHandler
{
    private readonly ICouponRepository _couponRepository;
    private readonly ILogger<CouponHandlers> _logger;

    public CouponHandlers(ICouponRepository couponRepository, ILogger<CouponHandlers> logger)
    {
        _couponRepository = couponRepository;
        _logger = logger;
    }

    public async Task<Result<CreateCouponResponse>> HandleAsync(CreateCouponCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await _couponRepository.GetByCodeAsync(command.Code, cancellationToken);
            if (existing != null)
                return Result<CreateCouponResponse>.Failure(new Error("Coupon.AlreadyExists", "Coupon code already exists."));

            var coupon = Coupon.Create(command.Code, command.Value, command.IsPercentage, command.ExpirationDate, command.UsageLimit, command.MinimumOrderValue);

            if (!coupon.IsValid)
                return Result<CreateCouponResponse>.Failure(new Error("Coupon.Validation", string.Join(", ", coupon.Notifications.Select(n => n.Message))));

            await _couponRepository.AddAsync(coupon, cancellationToken);

            return Result<CreateCouponResponse>.Success(new CreateCouponResponse(coupon.Id, coupon.Code, true, []));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coupon");
            return Result<CreateCouponResponse>.Failure(new Error("Coupon.CreateError", "Error creating coupon."));
        }
    }

    public async Task<Result<UpdateCouponResponse>> HandleAsync(UpdateCouponCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var coupon = await _couponRepository.GetByIdAsync(command.CouponId, cancellationToken);
            if (coupon == null)
                return Result<UpdateCouponResponse>.Failure(new Error("Coupon.NotFound", "Coupon not found."));

            if (command.Code != null) coupon.UpdateCode(command.Code);
            if (command.Value.HasValue) coupon.UpdateValue((int)command.Value.Value);
            if (command.IsPercentage.HasValue) coupon.UpdateIsPercentage(command.IsPercentage.Value);
            if (command.ExpirationDate.HasValue) coupon.UpdateExpirationDate(command.ExpirationDate.Value);
            if (command.UsageLimit.HasValue) coupon.UpdateUsageLimit(command.UsageLimit.Value);
            if (command.MinimumOrderValue.HasValue) coupon.UpdateMinimumOrderValue(command.MinimumOrderValue.Value);
            if (command.Active.HasValue) coupon.UpdateActive(command.Active.Value);

            if (!coupon.IsValid)
                return Result<UpdateCouponResponse>.Failure(new Error("Coupon.Validation", string.Join(", ", coupon.Notifications.Select(n => n.Message))));

            await _couponRepository.UpdateAsync(coupon, cancellationToken);

            return Result<UpdateCouponResponse>.Success(new UpdateCouponResponse(coupon.Id, coupon.Code, true, []));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating coupon");
            return Result<UpdateCouponResponse>.Failure(new Error("Coupon.UpdateError", "Error updating coupon."));
        }
    }

    public async Task<Result<CancelCouponResponse>> HandleAsync(CancelCouponCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var coupon = await _couponRepository.GetByIdAsync(command.CouponId, cancellationToken);
            if (coupon == null)
                return Result<CancelCouponResponse>.Failure(new Error("Coupon.NotFound", "Coupon not found."));

            coupon.UpdateActive(false);
            await _couponRepository.UpdateAsync(coupon, cancellationToken);

            return Result<CancelCouponResponse>.Success(new CancelCouponResponse(command.CouponId, true, []));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling coupon");
            return Result<CancelCouponResponse>.Failure(new Error("Coupon.CancelError", "Error cancelling coupon."));
        }
    }
}
