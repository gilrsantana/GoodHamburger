using GoodHamburger.Domain.Repositories;
using GoodHamburger.Shared.Entities.Locations;

namespace GoodHamburger.Domain.Repositories.Locations;

public interface ICountryRepository : IRepository<Country>
{
    Task<Country?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Country?> GetByIsoCodeAsync(string isoCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Country>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Country>> GetOrderedByNameAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Country>> GetCountriesWithStatesAsync(CancellationToken cancellationToken = default);
}
