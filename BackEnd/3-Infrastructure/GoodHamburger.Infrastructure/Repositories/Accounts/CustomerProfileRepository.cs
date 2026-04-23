using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Accounts.Entities;
using GoodHamburger.Domain.Repositories.Accounts;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories.Accounts;

public class CustomerProfileRepository : BaseRepository<CustomerProfile>, ICustomerProfileRepository
{
    public CustomerProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<CustomerProfile?> GetByDocumentAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Document.Number == documentNumber, cancellationToken);
    }

    public async Task<CustomerProfile?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.IdentityId == identityId, cancellationToken);
    }

    public async Task<IReadOnlyList<CustomerProfile>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CustomerProfile>> GetCustomersByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.FullName.Contains(name) || c.DisplayName.Contains(name))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(CustomerProfile entity, CancellationToken cancellationToken = default)
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
