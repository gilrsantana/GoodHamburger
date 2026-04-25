using GoodHamburger.Domain.Ordering.Entities;

namespace GoodHamburger.Domain.Repositories.Ordering;

public interface IOrderItemRepository : IRepository<OrderItem>
{
    Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItem>> GetByMenuItemIdAsync(Guid menuItemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItem>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItem>> GetItemsWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default);
}
