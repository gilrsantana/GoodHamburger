using GoodHamburger.Domain.Catalog.Entities;

namespace GoodHamburger.Domain.Repositories.Catalog;

public interface IIngredientRepository : IRepository<Ingredient>
{
    Task<Ingredient?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ingredient>> GetActiveIngredientsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ingredient>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ingredient>> GetIngredientsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
}
