using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Domain.Repositories.Catalog;

public interface IMenuItemRepository : IRepository<MenuItem>
{
    Task<MenuItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<MenuItem?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> GetActiveItemsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> GetAvailableItemsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> GetItemsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
}
