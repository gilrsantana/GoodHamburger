using GoodHamburger.Domain.Catalog.Entities;

namespace GoodHamburger.Domain.Repositories.Catalog;

public interface IMenuItemIngredientRepository
{
    Task<IReadOnlyList<MenuItemIngredient>> GetByMenuItemIdAsync(Guid menuItemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItemIngredient>> GetByIngredientIdAsync(Guid ingredientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItemIngredient>> GetRemovableIngredientsAsync(Guid menuItemId, CancellationToken cancellationToken = default);
    Task<MenuItemIngredient?> GetByMenuItemAndIngredientAsync(Guid menuItemId, Guid ingredientId, CancellationToken cancellationToken = default);
    Task<MenuItemIngredient> AddAsync(MenuItemIngredient entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuItemIngredient entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid menuItemId, Guid ingredientId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
