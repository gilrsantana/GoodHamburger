using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Catalog;

public class IngredientRepository : BaseRepository<Ingredient>, IIngredientRepository
{
    public IngredientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Ingredient?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Ingredient>> GetActiveIngredientsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Ingredient>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.Name.Contains(searchTerm) || i.Name.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Ingredient>> GetIngredientsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.SalePrice >= minPrice && i.SalePrice <= maxPrice)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Ingredient entity, CancellationToken cancellationToken = default)
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
