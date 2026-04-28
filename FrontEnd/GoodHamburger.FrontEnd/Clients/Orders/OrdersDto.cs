using System.Text.Json.Serialization;

namespace GoodHamburger.FrontEnd.Clients.Orders;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus { Pending, Confirmed, InPreparation, Ready, InDelivery, Completed, Cancelled, Failed }

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentType { Cpf, Cnpj, Passport }

public class Address
{
    public Guid StreetTypeId { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public Guid NeighborhoodId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
}

public record OrderItemExtraCommand(Guid IngredientId, string Name, double? Price);
public record OrderItemCommand(Guid MenuItemId, int Quantity, string? Note, IEnumerable<OrderItemExtraCommand>? Extras);

public class CreateOrderCommand 
{ 
    public CreateOrderCommand() { }
    public CreateOrderCommand(Guid customerId, Address deliveryAddress, double deliveryFee, IEnumerable<OrderItemCommand> items, string? couponCode)
    {
        CustomerId = customerId; DeliveryAddress = deliveryAddress; DeliveryFee = deliveryFee; Items = items; CouponCode = couponCode;
    }
    public Guid CustomerId { get; set; } 
    public Address DeliveryAddress { get; set; } = new(); 
    public double DeliveryFee { get; set; } 
    public IEnumerable<OrderItemCommand> Items { get; set; } = []; 
    public string? CouponCode { get; set; } 
}

public record CreateOrderResponse(Guid OrderId, double TotalItemsPrice, double TotalDiscounts, double FinalAmount, bool Succeeded, IEnumerable<string>? Errors);

public record OrderItemDetailDto(Guid IngredientId, string Name, double Price);
public record OrderItemDto(Guid Id, Guid MenuItemId, string ProductName, string Sku, double UnitPrice, int Quantity, double TotalItemPrice, string? Note, IEnumerable<OrderItemDetailDto> Details);
public record OrderDiscountDto(string Name, double Amount, string? CouponCode);

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public string? CancelReason { get; set; }
    public Address DeliveryAddress { get; set; } = new();
    public double DeliveryFee { get; set; }
    public double TotalItemsPrice { get; set; }
    public double TotalDiscounts { get; set; }
    public double FinalAmount { get; set; }
    public IEnumerable<OrderItemDto> Items { get; set; } = [];
    public IEnumerable<OrderDiscountDto> Discounts { get; set; } = [];
}

public record GetAllOrdersResponse(IEnumerable<OrderDto> Items, int TotalCount, int Page, int PageSize);
public record GetOrderByIdResponse(OrderDto? Order);
public record GetOrdersByCustomerResponse(IEnumerable<OrderDto> Items, int TotalCount, int Page, int PageSize);
public record GetOrdersByStatusResponse(IEnumerable<OrderDto> Items, int TotalCount, int Page, int PageSize);

public class UpdateOrderStatusCommand 
{ 
    public UpdateOrderStatusCommand() { }
    public UpdateOrderStatusCommand(Guid orderId, OrderStatus status, string? cancelReason)
    {
        OrderId = orderId; Status = status; CancelReason = cancelReason;
    }
    public Guid OrderId { get; set; } 
    public OrderStatus Status { get; set; } 
    public string? CancelReason { get; set; } 
}

public record UpdateOrderStatusResponse(Guid OrderId, OrderStatus NewStatus, bool Succeeded, IEnumerable<string>? Errors);
public record CancelOrderResponse(Guid OrderId, bool Succeeded, IEnumerable<string>? Errors);

// Checkout Calculate DTOs
public record CheckoutCalculationQuery(
    IReadOnlyList<CheckoutItemDto> Items,
    string? CouponCode = null,
    decimal? DeliveryFee = null
);

public record CheckoutItemDto(
    Guid MenuItemId,
    int Quantity,
    string? Note = null,
    IReadOnlyList<CheckoutExtraDto>? Extras = null
);

public record CheckoutExtraDto(
    Guid IngredientId,
    string Name,
    double? Price
);

public record CheckoutCalculationResponse(
    decimal Subtotal,
    decimal DeliveryFee,
    decimal DiscountAmount,
    decimal Total,
    IReadOnlyList<CheckoutItemResponseDto> Items,
    DiscountDto? AppliedDiscount
);

public record CheckoutItemResponseDto(
    Guid MenuItemId,
    string ProductName,
    string Sku,
    decimal UnitPrice,
    int Quantity,
    decimal TotalItemPrice,
    string? Note,
    IReadOnlyList<CheckoutExtraDto> Extras
);

public record DiscountDto(
    string Name,
    decimal Amount,
    string? CouponCode
);

public record ViaCepAddressDto(
    string ZipCode,
    string Street,
    string Neighborhood,
    string City,
    string State
);
