using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Ordering.Entities;
using GoodHamburger.Domain.Repositories.Ordering;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Ordering;

public class OrderItemDetailRepository : BaseRepository<OrderItemDetail>, IOrderItemDetailRepository
{
    public OrderItemDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<OrderItemDetail>> GetByOrderItemIdAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(oid => oid.OrderItemId == orderItemId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItemDetail>> GetByIngredientIdAsync(Guid ingredientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(oid => oid.IngredientId == ingredientId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItemDetail>> GetExtrasAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(oid => oid.OrderItemId == orderItemId && !oid.IsRemoved)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItemDetail>> GetRemovalsAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(oid => oid.OrderItemId == orderItemId && oid.IsRemoved)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(OrderItemDetail entity, CancellationToken cancellationToken = default)
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
