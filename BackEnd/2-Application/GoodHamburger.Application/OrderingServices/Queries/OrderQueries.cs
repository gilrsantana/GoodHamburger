using GoodHamburger.Domain.Ordering.Enums;
using GoodHamburger.Shared.ValueObjects.Locations;

namespace GoodHamburger.Application.OrderingServices.Queries;

public record GetAllOrdersQuery(int Page = 1, int PageSize = 10);

public record GetAllOrdersResponse(
    IReadOnlyList<OrderDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetOrderByIdQuery(Guid Id);

public record GetOrderByIdResponse(OrderDto? Order);

public record GetOrdersByCustomerQuery(Guid CustomerId, int Page = 1, int PageSize = 10);

public record GetOrdersByCustomerResponse(
    IReadOnlyList<OrderDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetOrdersByStatusQuery(OrderStatus Status, int Page = 1, int PageSize = 10);

public record GetOrdersByStatusResponse(
    IReadOnlyList<OrderDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    DateTime OrderDate,
    OrderStatus Status,
    string? CancelReason,
    Address DeliveryAddress,
    decimal DeliveryFee,
    decimal TotalItemsPrice,
    decimal TotalDiscounts,
    decimal FinalAmount,
    IReadOnlyList<OrderItemDto> Items,
    IReadOnlyList<OrderDiscountDto> Discounts
);

public record OrderItemDto(
    Guid Id,
    Guid MenuItemId,
    string ProductName,
    string Sku,
    decimal UnitPrice,
    int Quantity,
    decimal TotalItemPrice,
    string? Note,
    IReadOnlyList<OrderItemDetailDto> Details
);

public record OrderItemDetailDto(
    Guid IngredientId,
    string Name,
    decimal Price
);

public record OrderDiscountDto(
    string Name,
    decimal Amount,
    string? CouponCode
);
