using GoodHamburger.Domain.Repositories.Locations;
using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Locations;

public class NeighborhoodRepository : BaseRepository<Neighborhood>, INeighborhoodRepository
{
    public NeighborhoodRepository(DbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Neighborhood>> GetByCityIdAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.CityId == cityId)
            .Include(n => n.City)
            .ToListAsync(cancellationToken);
    }

    public async Task<Neighborhood?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(n => n.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Neighborhood>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.Name.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<Neighborhood?> GetByNameAndCityAsync(string name, Guid cityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(n => n.Name == name && n.CityId == cityId, cancellationToken);
    }
}
