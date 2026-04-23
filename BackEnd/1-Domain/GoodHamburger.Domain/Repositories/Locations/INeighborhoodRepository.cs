using GoodHamburger.Domain.Repositories;
using GoodHamburger.Shared.Entities.Locations;

namespace GoodHamburger.Domain.Repositories.Locations;

public interface INeighborhoodRepository : IRepository<Neighborhood>
{
    Task<IReadOnlyList<Neighborhood>> GetByCityIdAsync(Guid cityId, CancellationToken cancellationToken = default);
    Task<Neighborhood?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Neighborhood>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Neighborhood?> GetByNameAndCityAsync(string name, Guid cityId, CancellationToken cancellationToken = default);
}
