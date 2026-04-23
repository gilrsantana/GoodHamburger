using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Catalog.Enums;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Catalog;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(DbContext context) : base(context)
    {
    }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetByTypeAsync(MenuCategoryType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Type == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetOrderedByDisplayAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Category entity, CancellationToken cancellationToken = default)
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
