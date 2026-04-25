using GoodHamburger.Application.Services.Ordering.Contracts;
using GoodHamburger.Domain.Catalog.Enums;
using GoodHamburger.Domain.Ordering.Entities;
using GoodHamburger.Domain.Ordering.Enums;
using GoodHamburger.Domain.Repositories.Catalog;
using GoodHamburger.Domain.Repositories.Ordering;
using GoodHamburger.Shared.Handlers;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.OrderingServices.Commands;

public interface ICreateOrderHandler
{
    Task<Result<CreateOrderResponse>> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateOrderStatusHandler
{
    Task<Result<UpdateOrderStatusResponse>> HandleAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken = default);
}

public interface ICancelOrderHandler
{
    Task<Result<CancelOrderResponse>> HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken = default);
}

public class CreateOrderHandler : ICreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IOrderDiscountService _discountService;
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(
        IOrderRepository orderRepository,
        IMenuItemRepository menuItemRepository,
        ICouponRepository couponRepository,
        IOrderDiscountService discountService,
        ILogger<CreateOrderHandler> logger)
    {
        _orderRepository = orderRepository;
        _menuItemRepository = menuItemRepository;
        _couponRepository = couponRepository;
        _discountService = discountService;
        _logger = logger;
    }

    public async Task<Result<CreateOrderResponse>> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = new Order(command.CustomerId, command.DeliveryAddress, command.DeliveryFee);

            // Validação de regra: Apenas um item por categoria (Burger, Batata, Bebida)
            var categoriesAdded = new HashSet<MenuCategoryType>();

            foreach (var itemCommand in command.Items)
            {
                var menuItem = await _menuItemRepository.GetByIdAsync(itemCommand.MenuItemId, cancellationToken);
                if (menuItem == null)
                {
                    return Result<CreateOrderResponse>.Failure(new Error("Order.MenuItemNotFound", $"MenuItem {itemCommand.MenuItemId} not found."));
                }

                if (!menuItem.IsAvailable || !menuItem.Active)
                {
                    return Result<CreateOrderResponse>.Failure(new Error("Order.MenuItemUnavailable", $"MenuItem {menuItem.Name} is unavailable."));
                }

                if (menuItem.Category != null)
                {
                    if (categoriesAdded.Contains(menuItem.Category.Type))
                    {
                        return Result<CreateOrderResponse>.Failure(new Error("Order.DuplicateCategory", 
                            $"Each order can contain only one {menuItem.Category.Type}. Item '{menuItem.Name}' is duplicated or from the same category."));
                    }
                    categoriesAdded.Add(menuItem.Category.Type);
                }

                var orderItem = new OrderItem(
                    menuItem.Id,
                    menuItem.Name,
                    menuItem.Sku,
                    itemCommand.Quantity,
                    menuItem.Price,
                    itemCommand.Note
                );

                if (itemCommand.Extras != null)
                {
                    foreach (var extra in itemCommand.Extras)
                    {
                        orderItem.AddExtra(extra.IngredientId, extra.Name, extra.Price);
                    }
                }

                order.AddItem(orderItem);
            }

            // Aplicar Descontos de Combo (CategoryDiscountService)
            var bestDiscount = _discountService.CalculateBestDiscount(order);
            if (bestDiscount != null)
            {
                order.ApplyDiscount(bestDiscount);
            }

            // Aplicar Cupom se fornecido
            if (!string.IsNullOrWhiteSpace(command.CouponCode))
            {
                var coupon = await _couponRepository.GetByCodeAsync(command.CouponCode, cancellationToken);
                if (coupon != null)
                {
                    order.ApplyCoupon(coupon);
                }
            }

            if (!order.IsValid)
            {
                return Result<CreateOrderResponse>.Failure(new Error("Order.Validation", string.Join(", ", order.Notifications.Select(n => n.Message))));
            }

            await _orderRepository.AddAsync(order, cancellationToken);

            return Result<CreateOrderResponse>.Success(new CreateOrderResponse(
                order.Id,
                order.TotalItemsPrice,
                order.TotalDiscounts,
                order.FinalAmount,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return Result<CreateOrderResponse>.Failure(new Error("Order.CreateError", "An unexpected error occurred while creating the order."));
        }
    }
}

public class UpdateOrderStatusHandler : IUpdateOrderStatusHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<UpdateOrderStatusHandler> _logger;

    public UpdateOrderStatusHandler(IOrderRepository orderRepository, ILogger<UpdateOrderStatusHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateOrderStatusResponse>> HandleAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
            if (order == null)
                return Result<UpdateOrderStatusResponse>.Failure(new Error("Order.NotFound", "Order not found."));

            switch (command.Status)
            {
                case OrderStatus.Confirmed: order.ConfirmPayment(); break;
                case OrderStatus.InPreparation: order.StartPreparation(); break;
                case OrderStatus.Ready: order.SetAsReady(); break;
                case OrderStatus.InDelivery: order.Dispatch(); break;
                case OrderStatus.Completed: order.Complete(); break;
                case OrderStatus.Cancelled: order.Cancel(command.CancelReason ?? "No reason provided"); break;
                default:
                    return Result<UpdateOrderStatusResponse>.Failure(new Error("Order.InvalidStatus", "Invalid status transition."));
            }

            await _orderRepository.UpdateAsync(order, cancellationToken);

            return Result<UpdateOrderStatusResponse>.Success(new UpdateOrderStatusResponse(order.Id, order.Status, true, []));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status");
            return Result<UpdateOrderStatusResponse>.Failure(new Error("Order.UpdateError", "Error updating order status."));
        }
    }
}

public class CancelOrderHandler : ICancelOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CancelOrderHandler> _logger;

    public CancelOrderHandler(IOrderRepository orderRepository, ILogger<CancelOrderHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Result<CancelOrderResponse>> HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
            if (order == null)
                return Result<CancelOrderResponse>.Failure(new Error("Order.NotFound", "Order not found."));

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                return Result<CancelOrderResponse>.Failure(new Error("Order.CancelNotAllowed", "Completed or already cancelled orders cannot be cancelled."));

            order.Cancel(command.Reason);
            await _orderRepository.UpdateAsync(order, cancellationToken);

            return Result<CancelOrderResponse>.Success(new CancelOrderResponse(command.OrderId, true, []));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order");
            return Result<CancelOrderResponse>.Failure(new Error("Order.CancelError", "Error cancelling order."));
        }
    }
}
