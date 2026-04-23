using GoodHamburger.Domain.Ordering.Entities;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Domain.Repositories.Ordering;

public interface IOrderItemDetailRepository : IRepository<OrderItemDetail>
{
    Task<IReadOnlyList<OrderItemDetail>> GetByOrderItemIdAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItemDetail>> GetByIngredientIdAsync(Guid ingredientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItemDetail>> GetExtrasAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItemDetail>> GetRemovalsAsync(Guid orderItemId, CancellationToken cancellationToken = default);
}
