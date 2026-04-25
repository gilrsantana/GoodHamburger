using GoodHamburger.Domain.Repositories.Ordering;
using GoodHamburger.Shared.Handlers;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.OrderingServices.Queries;

public interface IGetAllOrdersHandler
{
    Task<Result<GetAllOrdersResponse>> HandleAsync(GetAllOrdersQuery query, CancellationToken cancellationToken = default);
}

public interface IGetOrderByIdHandler
{
    Task<Result<GetOrderByIdResponse>> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default);
}

public interface IGetOrdersByCustomerHandler
{
    Task<Result<GetOrdersByCustomerResponse>> HandleAsync(GetOrdersByCustomerQuery query, CancellationToken cancellationToken = default);
}

public interface IGetOrdersByStatusHandler
{
    Task<Result<GetOrdersByStatusResponse>> HandleAsync(GetOrdersByStatusQuery query, CancellationToken cancellationToken = default);
}

public class OrderQueryHandlers : IGetAllOrdersHandler, IGetOrderByIdHandler, IGetOrdersByCustomerHandler, IGetOrdersByStatusHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderQueryHandlers> _logger;

    public OrderQueryHandlers(IOrderRepository orderRepository, ILogger<OrderQueryHandlers> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Result<GetAllOrdersResponse>> HandleAsync(GetAllOrdersQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);
            var totalCount = orders.Count;
            var items = orders
                .OrderByDescending(o => o.OrderDate)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            return Result<GetAllOrdersResponse>.Success(new GetAllOrdersResponse(items, totalCount, query.Page, query.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying orders");
            return Result<GetAllOrdersResponse>.Failure(new Error("Order.QueryError", "Error retrieving orders."));
        }
    }

    public async Task<Result<GetOrderByIdResponse>> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(query.Id, cancellationToken);
            return Result<GetOrderByIdResponse>.Success(new GetOrderByIdResponse(order != null ? MapToDto(order) : null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying order {OrderId}", query.Id);
            return Result<GetOrderByIdResponse>.Failure(new Error("Order.QueryError", "Error retrieving order."));
        }
    }

    public async Task<Result<GetOrdersByCustomerResponse>> HandleAsync(GetOrdersByCustomerQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);
            var totalCount = orders.Count;
            var items = orders
                .OrderByDescending(o => o.OrderDate)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            return Result<GetOrdersByCustomerResponse>.Success(new GetOrdersByCustomerResponse(items, totalCount, query.Page, query.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying orders for customer {CustomerId}", query.CustomerId);
            return Result<GetOrdersByCustomerResponse>.Failure(new Error("Order.QueryError", "Error retrieving customer orders."));
        }
    }

    public async Task<Result<GetOrdersByStatusResponse>> HandleAsync(GetOrdersByStatusQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var orders = await _orderRepository.GetByStatusAsync(query.Status, cancellationToken);
            var totalCount = orders.Count;
            var items = orders
                .OrderByDescending(o => o.OrderDate)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            return Result<GetOrdersByStatusResponse>.Success(new GetOrdersByStatusResponse(items, totalCount, query.Page, query.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying orders by status {Status}", query.Status);
            return Result<GetOrdersByStatusResponse>.Failure(new Error("Order.QueryError", "Error retrieving orders by status."));
        }
    }

    private static OrderDto MapToDto(Domain.Ordering.Entities.Order order)
    {
        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.OrderDate,
            order.Status,
            order.CancelReason,
            order.DeliveryAddress,
            order.DeliveryFee,
            order.TotalItemsPrice,
            order.TotalDiscounts,
            order.FinalAmount,
            order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.MenuItemId,
                i.ProductName,
                i.Sku,
                i.UnitPrice,
                i.Quantity,
                i.TotalItemPrice,
                i.Note,
                i.Details.Select(d => new OrderItemDetailDto(d.IngredientId, d.Name, d.Price)).ToList()
            )).ToList(),
            order.Discounts.Select(d => new OrderDiscountDto(d.Name, d.Amount, d.CouponCode)).ToList()
        );
    }
}
