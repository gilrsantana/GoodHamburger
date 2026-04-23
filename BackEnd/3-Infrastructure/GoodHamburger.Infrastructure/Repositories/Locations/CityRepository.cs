using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Repositories.Locations;
using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Locations;

public class CityRepository : BaseRepository<City>, ICityRepository
{
    public CityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<City>> GetByStateIdAsync(Guid stateId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.StateId == stateId)
            .Include(c => c.State)
            .ToListAsync(cancellationToken);
    }

    public async Task<City?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<City>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Name.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<City?> GetByNameAndStateAsync(string name, Guid stateId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name == name && c.StateId == stateId, cancellationToken);
    }

    public async Task<IReadOnlyList<City>> GetCitiesWithNeighborhoodsAsync(Guid stateId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.StateId == stateId)
            .Include(c => c.Neighborhoods)
            .ToListAsync(cancellationToken);
    }
}
