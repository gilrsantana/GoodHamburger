using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Catalog;

public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MenuItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.Sku == sku, cancellationToken);
    }

    public async Task<MenuItem?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetActiveItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetAvailableItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.Active && m.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.Name.Contains(searchTerm) || m.Description.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetItemsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.Price >= minPrice && m.Price <= maxPrice)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuItem entity, CancellationToken cancellationToken = default)
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
