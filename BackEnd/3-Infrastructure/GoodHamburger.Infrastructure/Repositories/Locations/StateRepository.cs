using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Repositories.Locations;
using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Locations;

public class StateRepository : BaseRepository<State>, IStateRepository
{
    public StateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<State>> GetByCountryIdAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.CountryId == countryId)
            .Include(s => s.Country)
            .ToListAsync(cancellationToken);
    }

    public async Task<State?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }

    public async Task<State?> GetByUfAsync(string uf, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.UF == uf.ToUpper(), cancellationToken);
    }

    public async Task<IReadOnlyList<State>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Name.Contains(searchTerm) || s.UF.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<State?> GetByNameAndCountryAsync(string name, Guid countryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name == name && s.CountryId == countryId, cancellationToken);
    }

    public async Task<IReadOnlyList<State>> GetStatesWithCitiesAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.CountryId == countryId)
            .Include(s => s.Cities)
            .ToListAsync(cancellationToken);
    }
}
