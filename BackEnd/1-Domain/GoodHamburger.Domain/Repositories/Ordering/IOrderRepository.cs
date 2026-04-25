using GoodHamburger.Domain.Ordering.Entities;
using GoodHamburger.Domain.Ordering.Enums;

namespace GoodHamburger.Domain.Repositories.Ordering;

public interface IOrderRepository : IRepository<Order>
{
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetActiveOrdersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetCompletedOrdersAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<int> GetOrderCountAsync(OrderStatus? status = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
}
