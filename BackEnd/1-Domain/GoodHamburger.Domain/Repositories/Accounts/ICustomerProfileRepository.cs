using GoodHamburger.Domain.Accounts.Entities;

namespace GoodHamburger.Domain.Repositories.Accounts;

public interface ICustomerProfileRepository : IRepository<CustomerProfile>
{
    Task<CustomerProfile?> GetByDocumentAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<CustomerProfile?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerProfile>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerProfile>> GetCustomersByNameAsync(string name, CancellationToken cancellationToken = default);
}
