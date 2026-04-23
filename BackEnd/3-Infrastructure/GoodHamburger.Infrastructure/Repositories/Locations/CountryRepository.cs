using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Repositories.Locations;
using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Locations;

public class CountryRepository : BaseRepository<Country>, ICountryRepository
{
    public CountryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Country?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<Country?> GetByIsoCodeAsync(string isoCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.IsoCode == isoCode.ToUpper(), cancellationToken);
    }

    public async Task<IReadOnlyList<Country>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Name.Contains(searchTerm) || c.IsoCode.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Country>> GetOrderedByNameAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Country>> GetCountriesWithStatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.States)
            .ToListAsync(cancellationToken);
    }
}
