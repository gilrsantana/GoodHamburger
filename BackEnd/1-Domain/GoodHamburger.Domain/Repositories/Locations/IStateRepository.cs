using GoodHamburger.Domain.Repositories;
using GoodHamburger.Shared.Entities.Locations;

namespace GoodHamburger.Domain.Repositories.Locations;

public interface IStateRepository : IRepository<State>
{
    Task<IReadOnlyList<State>> GetByCountryIdAsync(Guid countryId, CancellationToken cancellationToken = default);
    Task<State?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<State?> GetByUfAsync(string uf, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<State>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<State?> GetByNameAndCountryAsync(string name, Guid countryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<State>> GetStatesWithCitiesAsync(Guid countryId, CancellationToken cancellationToken = default);
}
