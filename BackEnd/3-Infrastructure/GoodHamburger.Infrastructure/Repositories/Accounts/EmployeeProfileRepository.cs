using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Accounts.Entities;
using GoodHamburger.Domain.Repositories.Accounts;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Accounts;

public class EmployeeProfileRepository : BaseRepository<EmployeeProfile>, IEmployeeProfileRepository
{
    public EmployeeProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<EmployeeProfile?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.IdentityId == identityId, cancellationToken);
    }

    public async Task<IReadOnlyList<EmployeeProfile>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmployeeProfile>> GetEmployeesByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.FullName.Contains(name) || e.DisplayName.Contains(name))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(EmployeeProfile entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
