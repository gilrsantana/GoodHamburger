using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.CatalogServices.Commands;
using GoodHamburger.Application.CatalogServices.Queries;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ApiController
{
    private readonly ICreateIngredientHandler _createIngredientHandler;
    private readonly IUpdateIngredientHandler _updateIngredientHandler;
    private readonly IDeleteIngredientHandler _deleteIngredientHandler;
    private readonly IActivateIngredientHandler _activateIngredientHandler;
    private readonly IDeactivateIngredientHandler _deactivateIngredientHandler;
    private readonly IGetAllIngredientsHandler _getAllIngredientsHandler;
    private readonly IGetIngredientByIdHandler _getIngredientByIdHandler;
    private readonly IGetActiveIngredientsHandler _getActiveIngredientsHandler;
    private readonly ISearchIngredientsHandler _searchIngredientsHandler;
    private readonly IGetIngredientsByPriceRangeHandler _getIngredientsByPriceRangeHandler;

    public IngredientsController(
        ICreateIngredientHandler createIngredientHandler,
        IUpdateIngredientHandler updateIngredientHandler,
        IDeleteIngredientHandler deleteIngredientHandler,
        IActivateIngredientHandler activateIngredientHandler,
        IDeactivateIngredientHandler deactivateIngredientHandler,
        IGetAllIngredientsHandler getAllIngredientsHandler,
        IGetIngredientByIdHandler getIngredientByIdHandler,
        IGetActiveIngredientsHandler getActiveIngredientsHandler,
        ISearchIngredientsHandler searchIngredientsHandler,
        IGetIngredientsByPriceRangeHandler getIngredientsByPriceRangeHandler)
    {
        _createIngredientHandler = createIngredientHandler;
        _updateIngredientHandler = updateIngredientHandler;
        _deleteIngredientHandler = deleteIngredientHandler;
        _activateIngredientHandler = activateIngredientHandler;
        _deactivateIngredientHandler = deactivateIngredientHandler;
        _getAllIngredientsHandler = getAllIngredientsHandler;
        _getIngredientByIdHandler = getIngredientByIdHandler;
        _getActiveIngredientsHandler = getActiveIngredientsHandler;
        _searchIngredientsHandler = searchIngredientsHandler;
        _getIngredientsByPriceRangeHandler = getIngredientsByPriceRangeHandler;
    }

    [HttpPost]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(CreateIngredientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientCommand command, CancellationToken cancellationToken)
    {
        var result = await _createIngredientHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateIngredientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIngredient(Guid id, [FromBody] UpdateIngredientCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { IngredientId = id };
        var result = await _updateIngredientHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(DeleteIngredientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIngredient(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteIngredientCommand(id);
        var result = await _deleteIngredientHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPost("{id}/activate")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(ActivateIngredientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateIngredient(Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateIngredientCommand(id);
        var result = await _activateIngredientHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(DeactivateIngredientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateIngredient(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateIngredientCommand(id);
        var result = await _deactivateIngredientHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetAllIngredientsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllIngredients(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllIngredientsQuery(page, pageSize);
        var result = await _getAllIngredientsHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetIngredientByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIngredientById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetIngredientByIdQuery(id);
        var result = await _getIngredientByIdHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetActiveIngredientsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetActiveIngredients(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetActiveIngredientsQuery(page, pageSize);
        var result = await _getActiveIngredientsHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SearchIngredientsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchIngredients(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchIngredientsQuery(searchTerm, page, pageSize);
        var result = await _searchIngredientsHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("price-range")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetIngredientsByPriceRangeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetIngredientsByPriceRange(
        [FromQuery] decimal minPrice,
        [FromQuery] decimal maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetIngredientsByPriceRangeQuery(minPrice, maxPrice, page, pageSize);
        var result = await _getIngredientsByPriceRangeHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
}
