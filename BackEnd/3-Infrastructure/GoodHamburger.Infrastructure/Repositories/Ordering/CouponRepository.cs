using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Ordering.Entities;
using GoodHamburger.Domain.Repositories.Ordering;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Ordering;

public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
{
    public CouponRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Coupon>> GetActiveCouponsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Active && c.ExpirationDate > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Coupon>> GetExpiredCouponsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.ExpirationDate <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Coupon>> GetCouponsByUsageRangeAsync(int minUsage, int maxUsage, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.UsageCount >= minUsage && c.UsageCount <= maxUsage)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Coupon entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
