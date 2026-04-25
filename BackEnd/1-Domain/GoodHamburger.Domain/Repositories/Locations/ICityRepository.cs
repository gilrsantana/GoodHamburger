using GoodHamburger.Shared.Entities.Locations;

namespace GoodHamburger.Domain.Repositories.Locations;

public interface ICityRepository : IRepository<City>
{
    Task<IReadOnlyList<City>> GetByStateIdAsync(Guid stateId, CancellationToken cancellationToken = default);
    Task<City?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<City>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<City?> GetByNameAndStateAsync(string name, Guid stateId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<City>> GetCitiesWithNeighborhoodsAsync(Guid stateId, CancellationToken cancellationToken = default);
}
