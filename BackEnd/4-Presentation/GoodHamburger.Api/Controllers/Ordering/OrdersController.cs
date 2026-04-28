using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.OrderingServices.Commands;
using GoodHamburger.Application.OrderingServices.Queries;
using GoodHamburger.Domain.Ordering.Enums;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Ordering;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : BaseApiController
{
    private readonly ICreateOrderHandler _createOrderHandler;
    private readonly IUpdateOrderStatusHandler _updateOrderStatusHandler;
    private readonly ICancelOrderHandler _cancelOrderHandler;
    private readonly IGetAllOrdersHandler _getAllOrdersHandler;
    private readonly IGetOrderByIdHandler _getOrderByIdHandler;
    private readonly IGetOrdersByCustomerHandler _getOrdersByCustomerHandler;
    private readonly IGetOrdersByStatusHandler _getOrdersByStatusHandler;
    private readonly ICheckoutCalculationHandler _checkoutCalculationHandler;

    public OrdersController(
        ICreateOrderHandler createOrderHandler,
        IUpdateOrderStatusHandler updateOrderStatusHandler,
        ICancelOrderHandler cancelOrderHandler,
        IGetAllOrdersHandler getAllOrdersHandler,
        IGetOrderByIdHandler getOrderByIdHandler,
        IGetOrdersByCustomerHandler getOrdersByCustomerHandler,
        IGetOrdersByStatusHandler getOrdersByStatusHandler,
        ICheckoutCalculationHandler checkoutCalculationHandler)
    {
        _createOrderHandler = createOrderHandler;
        _updateOrderStatusHandler = updateOrderStatusHandler;
        _cancelOrderHandler = cancelOrderHandler;
        _getAllOrdersHandler = getAllOrdersHandler;
        _getOrderByIdHandler = getOrderByIdHandler;
        _getOrdersByCustomerHandler = getOrdersByCustomerHandler;
        _getOrdersByStatusHandler = getOrdersByStatusHandler;
        _checkoutCalculationHandler = checkoutCalculationHandler;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _createOrderHandler.HandleAsync(command, cancellationToken);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetOrderById), new { id = result.Value?.OrderId }, result.Value) 
            : HandleFailure(result);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateOrderStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        var updatedCommand = command with { OrderId = id };
        var result = await _updateOrderStatusHandler.HandleAsync(updatedCommand, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost("{id}/cancel")]
    [Authorize]
    [ProducesResponseType(typeof(CancelOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder(Guid id, [FromBody] string reason, CancellationToken cancellationToken)
    {
        var result = await _cancelOrderHandler.HandleAsync(new CancelOrderCommand(id, reason), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetAllOrdersResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _getAllOrdersHandler.HandleAsync(new GetAllOrdersQuery(page, pageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(GetOrderByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getOrderByIdHandler.HandleAsync(new GetOrderByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("customer/{customerId}")]
    [Authorize]
    [ProducesResponseType(typeof(GetOrdersByCustomerResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _getOrdersByCustomerHandler.HandleAsync(new GetOrdersByCustomerQuery(customerId, page, pageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("status/{status}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetOrdersByStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStatus(OrderStatus status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _getOrdersByStatusHandler.HandleAsync(new GetOrdersByStatusQuery(status, page, pageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost("checkout/calculate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CheckoutCalculationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalculateCheckout([FromBody] CheckoutCalculationQuery query, CancellationToken cancellationToken)
    {
        var result = await _checkoutCalculationHandler.HandleAsync(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }
}
