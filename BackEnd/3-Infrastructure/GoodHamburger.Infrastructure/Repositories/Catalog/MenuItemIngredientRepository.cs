using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Catalog;

public class MenuItemIngredientRepository : IMenuItemIngredientRepository
{
    private readonly DbContext _context;
    private readonly DbSet<MenuItemIngredient> _dbSet;

    public MenuItemIngredientRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<MenuItemIngredient>();
    }

    public async Task<IReadOnlyList<MenuItemIngredient>> GetByMenuItemIdAsync(Guid menuItemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(mii => mii.MenuItemId == menuItemId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItemIngredient>> GetByIngredientIdAsync(Guid ingredientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(mii => mii.IngredientId == ingredientId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItemIngredient>> GetRemovableIngredientsAsync(Guid menuItemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(mii => mii.MenuItemId == menuItemId && mii.IsRemovable)
            .ToListAsync(cancellationToken);
    }

    public async Task<MenuItemIngredient?> GetByMenuItemAndIngredientAsync(Guid menuItemId, Guid ingredientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(mii => mii.MenuItemId == menuItemId && mii.IngredientId == ingredientId, cancellationToken);
    }

    public async Task<MenuItemIngredient> AddAsync(MenuItemIngredient entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(MenuItemIngredient entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuItemIngredient entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid menuItemId, Guid ingredientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(mii => mii.MenuItemId == menuItemId && mii.IngredientId == ingredientId, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }
}
