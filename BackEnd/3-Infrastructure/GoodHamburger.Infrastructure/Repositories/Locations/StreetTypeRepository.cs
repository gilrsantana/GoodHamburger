using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Repositories.Locations;
using GoodHamburger.Shared.Entities.Locations;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Locations;

public class StreetTypeRepository : BaseRepository<StreetType>, IStreetTypeRepository
{
    public StreetTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<StreetType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(st => st.Name == name, cancellationToken);
    }

    public async Task<StreetType?> GetByAbbreviationAsync(string abbreviation, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(st => st.Abbreviation == abbreviation, cancellationToken);
    }

    public async Task<IReadOnlyList<StreetType>> GetByNameContainingAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(st => st.Name.Contains(searchTerm) || st.Abbreviation.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StreetType>> GetOrderedByNameAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderBy(st => st.Name)
            .ToListAsync(cancellationToken);
    }
}
