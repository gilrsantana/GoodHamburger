using GoodHamburger.Domain.Repositories;
using GoodHamburger.Shared.Entities.Locations;

namespace GoodHamburger.Domain.Repositories.Locations;

public interface IStreetTypeRepository : IRepository<StreetType>
{
    Task<StreetType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<StreetType?> GetByAbbreviationAsync(string abbreviation, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StreetType>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StreetType>> GetOrderedByNameAsync(CancellationToken cancellationToken = default);
}
