using GoodHamburger.Domain.Ordering.Enums;
using GoodHamburger.Shared.ValueObjects.Locations;

namespace GoodHamburger.Application.OrderingServices.Commands;

public record CreateOrderCommand(
    Guid CustomerId,
    Address DeliveryAddress,
    decimal DeliveryFee,
    List<OrderItemCommand> Items,
    string? CouponCode = null
);

public record OrderItemCommand(
    Guid MenuItemId,
    int Quantity,
    string? Note = null,
    List<OrderItemExtraCommand>? Extras = null
);

public record OrderItemExtraCommand(
    Guid IngredientId,
    string Name,
    decimal Price
);

public record CreateOrderResponse(
    Guid OrderId,
    decimal TotalItemsPrice,
    decimal TotalDiscounts,
    decimal FinalAmount,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus Status,
    string? CancelReason = null
);

public record UpdateOrderStatusResponse(
    Guid OrderId,
    OrderStatus NewStatus,
    bool Succeeded,
    IEnumerable<string> Errors
);

public record CancelOrderCommand(Guid OrderId, string Reason);

public record CancelOrderResponse(
    Guid OrderId,
    bool Succeeded,
    IEnumerable<string> Errors
);
