using GoodHamburger.Application.Services.Ordering.Contracts;
using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Catalog.Enums;
using GoodHamburger.Domain.Repositories.Catalog;
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

public interface ICheckoutCalculationHandler
{
    Task<Result<CheckoutCalculationResponse>> HandleAsync(CheckoutCalculationQuery query, CancellationToken cancellationToken = default);
}

public class OrderQueryHandlers : IGetAllOrdersHandler, IGetOrderByIdHandler, IGetOrdersByCustomerHandler, IGetOrdersByStatusHandler, ICheckoutCalculationHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IOrderDiscountService _orderDiscountService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<OrderQueryHandlers> _logger;

    public OrderQueryHandlers(
        IOrderRepository orderRepository,
        IMenuItemRepository menuItemRepository,
        ICouponRepository couponRepository,
        IOrderDiscountService orderDiscountService,
        ILogger<OrderQueryHandlers> logger, ICategoryRepository categoryRepository)
    {
        _orderRepository = orderRepository;
        _menuItemRepository = menuItemRepository;
        _couponRepository = couponRepository;
        _orderDiscountService = orderDiscountService;
        _logger = logger;
        _categoryRepository = categoryRepository;
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

    public async Task<Result<CheckoutCalculationResponse>> HandleAsync(CheckoutCalculationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            if (query.Items == null || !query.Items.Any())
            {
                return Result<CheckoutCalculationResponse>.Failure(
                    new Error("Checkout.EmptyCart", "Cart cannot be empty."));
            }

            var itemResponses = new List<CheckoutItemResponseDto>();
            var itemsWithCategories = new List<(MenuCategoryType? categoryType, int quantity)>();
            decimal subtotal = 0;

            foreach (var item in query.Items)
            {
                var menuItem = await _menuItemRepository.GetByIdAsync(item.MenuItemId, cancellationToken);
                
                if (menuItem == null)
                {
                    return Result<CheckoutCalculationResponse>.Failure(
                        new Error("Checkout.ItemNotFound", $"Menu item with ID {item.MenuItemId} not found."));
                }

                await GetCategoryFromMenuItem(cancellationToken, menuItem);

                if (!menuItem.Active || !menuItem.IsAvailable)
                {
                    return Result<CheckoutCalculationResponse>.Failure(
                        new Error("Checkout.ItemUnavailable", $"Menu item '{menuItem.Name}' is not available."));
                }

                var extrasPrice = item.Extras?.Sum(e => e.Price) ?? 0;
                var unitPrice = menuItem.Price;
                var totalItemPrice = (unitPrice + extrasPrice) * item.Quantity;

                itemResponses.Add(new CheckoutItemResponseDto(
                    menuItem.Id,
                    menuItem.Name,
                    menuItem.Sku,
                    unitPrice,
                    item.Quantity,
                    totalItemPrice,
                    item.Note,
                    item.Extras ?? new List<CheckoutExtraDto>()
                ));

                itemsWithCategories.Add((menuItem.Category?.Type, item.Quantity));
                subtotal += totalItemPrice;
            }

            var deliveryFee = query.DeliveryFee ?? 0;
            decimal discountAmount = 0;
            DiscountDto? appliedDiscount = null;

            // Apply category-based discount using IOrderDiscountService
            var (discountAmountFromService, discountName) = _orderDiscountService.CalculateBestDiscountForCheckout(itemsWithCategories, subtotal);
            if (discountAmountFromService.HasValue && discountName != null)
            {
                discountAmount = discountAmountFromService.Value;
                appliedDiscount = new DiscountDto(discountName, discountAmount, null);
            }

            var total = subtotal - discountAmount + deliveryFee;

            return Result<CheckoutCalculationResponse>.Success(new CheckoutCalculationResponse(
                subtotal,
                deliveryFee,
                discountAmount,
                total,
                itemResponses,
                appliedDiscount
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating checkout totals");
            return Result<CheckoutCalculationResponse>.Failure(
                new Error("Checkout.CalculationError", "Error calculating checkout totals."));
        }
    }

    private async Task GetCategoryFromMenuItem(CancellationToken cancellationToken, MenuItem menuItem)
    {
        var category = await _categoryRepository.GetByIdAsync(menuItem.CategoryId, cancellationToken);
        if (category == null)
            return;

        menuItem.UpdateCategory(category);
    }
}
