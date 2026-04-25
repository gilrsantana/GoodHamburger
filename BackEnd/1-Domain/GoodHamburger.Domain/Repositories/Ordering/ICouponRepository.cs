using GoodHamburger.Domain.Ordering.Entities;

namespace GoodHamburger.Domain.Repositories.Ordering;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> GetActiveCouponsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> GetExpiredCouponsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> GetCouponsByUsageRangeAsync(int minUsage, int maxUsage, CancellationToken cancellationToken = default);
}
