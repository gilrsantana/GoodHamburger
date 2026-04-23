using GoodHamburger.Domain.Ordering.Entities;
using GoodHamburger.Domain.Repositories.Ordering;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Ordering;

public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(DbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.Details)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItem>> GetByMenuItemIdAsync(Guid menuItemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(oi => oi.MenuItemId == menuItemId)
            .Include(oi => oi.Details)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItem>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(oi => oi.Sku == sku)
            .Include(oi => oi.Details)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItem>> GetItemsWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.Details)
            .Include(oi => oi.MenuItem)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(OrderItem entity, CancellationToken cancellationToken = default)
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
