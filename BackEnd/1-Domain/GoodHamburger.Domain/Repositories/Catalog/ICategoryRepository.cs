using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Catalog.Enums;

namespace GoodHamburger.Domain.Repositories.Catalog;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetByTypeAsync(MenuCategoryType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetOrderedByDisplayAsync(CancellationToken cancellationToken = default);
}
