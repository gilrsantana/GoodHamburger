using GoodHamburger.Domain.Accounts.Entities;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Domain.Repositories.Accounts;

public interface IEmployeeProfileRepository : IRepository<EmployeeProfile>
{
    Task<EmployeeProfile?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeProfile>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeProfile>> GetEmployeesByNameAsync(string name, CancellationToken cancellationToken = default);
}
