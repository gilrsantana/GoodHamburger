using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.OrderingServices.Commands;
using GoodHamburger.Application.OrderingServices.Queries;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Ordering;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : BaseApiController
{
    private readonly ICreateCouponHandler _createCouponHandler;
    private readonly IUpdateCouponHandler _updateCouponHandler;
    private readonly ICancelCouponHandler _cancelCouponHandler;
    private readonly IGetAllCouponsHandler _getAllCouponsHandler;
    private readonly IGetCouponByIdHandler _getCouponByIdHandler;
    private readonly IGetCouponByCodeHandler _getCouponByCodeHandler;
    private readonly IGetActiveCouponsHandler _getActiveCouponsHandler;

    public CouponsController(
        ICreateCouponHandler createCouponHandler,
        IUpdateCouponHandler updateCouponHandler,
        ICancelCouponHandler cancelCouponHandler,
        IGetAllCouponsHandler getAllCouponsHandler,
        IGetCouponByIdHandler getCouponByIdHandler,
        IGetCouponByCodeHandler getCouponByCodeHandler,
        IGetActiveCouponsHandler getActiveCouponsHandler)
    {
        _createCouponHandler = createCouponHandler;
        _updateCouponHandler = updateCouponHandler;
        _cancelCouponHandler = cancelCouponHandler;
        _getAllCouponsHandler = getAllCouponsHandler;
        _getCouponByIdHandler = getCouponByIdHandler;
        _getCouponByCodeHandler = getCouponByCodeHandler;
        _getActiveCouponsHandler = getActiveCouponsHandler;
    }

    [HttpPost]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(CreateCouponResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponCommand command, CancellationToken cancellationToken)
    {
        var result = await _createCouponHandler.HandleAsync(command, cancellationToken);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetById), new { id = result.Value?.Id }, result.Value) 
            : HandleFailure(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateCouponResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCouponCommand command, CancellationToken cancellationToken)
    {
        var updatedCommand = command with { CouponId = id };
        var result = await _updateCouponHandler.HandleAsync(updatedCommand, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(CancelCouponResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _cancelCouponHandler.HandleAsync(new CancelCouponCommand(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetAllCouponsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _getAllCouponsHandler.HandleAsync(new GetAllCouponsQuery(page, pageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetCouponByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getCouponByIdHandler.HandleAsync(new GetCouponByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("code/{code}")]
    [Authorize]
    [ProducesResponseType(typeof(GetCouponByCodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken)
    {
        var result = await _getCouponByCodeHandler.HandleAsync(new GetCouponByCodeQuery(code), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetActiveCouponsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _getActiveCouponsHandler.HandleAsync(new GetActiveCouponsQuery(page, pageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }
}
