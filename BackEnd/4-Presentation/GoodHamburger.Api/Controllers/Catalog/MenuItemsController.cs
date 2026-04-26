using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.CatalogServices.Commands;
using GoodHamburger.Application.CatalogServices.Queries;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class MenuItemsController : BaseApiController
{
    private readonly ICreateMenuItemHandler _createMenuItemHandler;
    private readonly IUpdateMenuItemHandler _updateMenuItemHandler;
    private readonly IDeleteMenuItemHandler _deleteMenuItemHandler;
    private readonly IActivateMenuItemHandler _activateMenuItemHandler;
    private readonly IDeactivateMenuItemHandler _deactivateMenuItemHandler;
    private readonly ISetMenuItemAvailabilityHandler _setMenuItemAvailabilityHandler;
    private readonly IAddIngredientToMenuItemHandler _addIngredientToMenuItemHandler;
    private readonly IRemoveIngredientFromMenuItemHandler _removeIngredientFromMenuItemHandler;
    private readonly IUpdateIngredientRemovabilityHandler _updateIngredientRemovabilityHandler;
    private readonly IGetAllMenuItemsHandler _getAllMenuItemsHandler;
    private readonly IGetMenuItemByIdHandler _getMenuItemByIdHandler;
    private readonly IGetMenuItemBySkuHandler _getMenuItemBySkuHandler;
    private readonly IGetMenuItemsByCategoryHandler _getMenuItemsByCategoryHandler;
    private readonly IGetActiveMenuItemsHandler _getActiveMenuItemsHandler;
    private readonly IGetAvailableMenuItemsHandler _getAvailableMenuItemsHandler;
    private readonly ISearchMenuItemsHandler _searchMenuItemsHandler;
    private readonly IGetMenuItemsByPriceRangeHandler _getMenuItemsByPriceRangeHandler;

    public MenuItemsController(
        ICreateMenuItemHandler createMenuItemHandler,
        IUpdateMenuItemHandler updateMenuItemHandler,
        IDeleteMenuItemHandler deleteMenuItemHandler,
        IActivateMenuItemHandler activateMenuItemHandler,
        IDeactivateMenuItemHandler deactivateMenuItemHandler,
        ISetMenuItemAvailabilityHandler setMenuItemAvailabilityHandler,
        IAddIngredientToMenuItemHandler addIngredientToMenuItemHandler,
        IRemoveIngredientFromMenuItemHandler removeIngredientFromMenuItemHandler,
        IUpdateIngredientRemovabilityHandler updateIngredientRemovabilityHandler,
        IGetAllMenuItemsHandler getAllMenuItemsHandler,
        IGetMenuItemByIdHandler getMenuItemByIdHandler,
        IGetMenuItemBySkuHandler getMenuItemBySkuHandler,
        IGetMenuItemsByCategoryHandler getMenuItemsByCategoryHandler,
        IGetActiveMenuItemsHandler getActiveMenuItemsHandler,
        IGetAvailableMenuItemsHandler getAvailableMenuItemsHandler,
        ISearchMenuItemsHandler searchMenuItemsHandler,
        IGetMenuItemsByPriceRangeHandler getMenuItemsByPriceRangeHandler)
    {
        _createMenuItemHandler = createMenuItemHandler;
        _updateMenuItemHandler = updateMenuItemHandler;
        _deleteMenuItemHandler = deleteMenuItemHandler;
        _activateMenuItemHandler = activateMenuItemHandler;
        _deactivateMenuItemHandler = deactivateMenuItemHandler;
        _setMenuItemAvailabilityHandler = setMenuItemAvailabilityHandler;
        _addIngredientToMenuItemHandler = addIngredientToMenuItemHandler;
        _removeIngredientFromMenuItemHandler = removeIngredientFromMenuItemHandler;
        _updateIngredientRemovabilityHandler = updateIngredientRemovabilityHandler;
        _getAllMenuItemsHandler = getAllMenuItemsHandler;
        _getMenuItemByIdHandler = getMenuItemByIdHandler;
        _getMenuItemBySkuHandler = getMenuItemBySkuHandler;
        _getMenuItemsByCategoryHandler = getMenuItemsByCategoryHandler;
        _getActiveMenuItemsHandler = getActiveMenuItemsHandler;
        _getAvailableMenuItemsHandler = getAvailableMenuItemsHandler;
        _searchMenuItemsHandler = searchMenuItemsHandler;
        _getMenuItemsByPriceRangeHandler = getMenuItemsByPriceRangeHandler;
    }

    [HttpPost]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(CreateMenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemCommand command, CancellationToken cancellationToken)
    {
        var result = await _createMenuItemHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateMenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMenuItem(Guid id, [FromBody] UpdateMenuItemCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { MenuItemId = id };
        var result = await _updateMenuItemHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(DeleteMenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenuItem(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteMenuItemCommand(id);
        var result = await _deleteMenuItemHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPost("{id}/activate")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(ActivateMenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateMenuItem(Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateMenuItemCommand(id);
        var result = await _activateMenuItemHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(DeactivateMenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateMenuItem(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateMenuItemCommand(id);
        var result = await _deactivateMenuItemHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPost("{id}/availability")]
    [Authorize(Policy = "Operate")]
    [ProducesResponseType(typeof(SetMenuItemAvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetMenuItemAvailability(Guid id, [FromQuery] bool isAvailable, CancellationToken cancellationToken)
    {
        var command = new SetMenuItemAvailabilityCommand(id, isAvailable);
        var result = await _setMenuItemAvailabilityHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPost("{menuItemId}/ingredients/{ingredientId}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(AddIngredientToMenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddIngredientToMenuItem(
        Guid menuItemId,
        Guid ingredientId,
        [FromQuery] bool isRemovable = true,
        CancellationToken cancellationToken = default)
    {
        var command = new AddIngredientToMenuItemCommand(menuItemId, ingredientId, isRemovable);
        var result = await _addIngredientToMenuItemHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpDelete("{menuItemId}/ingredients/{ingredientId}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(RemoveIngredientFromMenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveIngredientFromMenuItem(
        Guid menuItemId,
        Guid ingredientId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveIngredientFromMenuItemCommand(menuItemId, ingredientId);
        var result = await _removeIngredientFromMenuItemHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{menuItemId}/ingredients/{ingredientId}/removability")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateIngredientRemovabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIngredientRemovability(
        Guid menuItemId,
        Guid ingredientId,
        [FromQuery] bool isRemovable,
        CancellationToken cancellationToken)
    {
        var command = new UpdateIngredientRemovabilityCommand(menuItemId, ingredientId, isRemovable);
        var result = await _updateIngredientRemovabilityHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetAllMenuItemsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllMenuItems(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllMenuItemsQuery(page, pageSize);
        var result = await _getAllMenuItemsHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetMenuItemByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItemById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetMenuItemByIdQuery(id);
        var result = await _getMenuItemByIdHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("sku/{sku}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetMenuItemBySkuResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItemBySku(string sku, CancellationToken cancellationToken)
    {
        var query = new GetMenuItemBySkuQuery(sku);
        var result = await _getMenuItemBySkuHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetMenuItemsByCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMenuItemsByCategory(
        Guid categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMenuItemsByCategoryQuery(categoryId, page, pageSize);
        var result = await _getMenuItemsByCategoryHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetActiveMenuItemsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetActiveMenuItems(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetActiveMenuItemsQuery(page, pageSize);
        var result = await _getActiveMenuItemsHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetAvailableMenuItemsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableMenuItems(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAvailableMenuItemsQuery(page, pageSize);
        var result = await _getAvailableMenuItemsHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SearchMenuItemsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchMenuItems(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchMenuItemsQuery(searchTerm, page, pageSize);
        var result = await _searchMenuItemsHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("price-range")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetMenuItemsByPriceRangeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMenuItemsByPriceRange(
        [FromQuery] decimal minPrice,
        [FromQuery] decimal maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMenuItemsByPriceRangeQuery(minPrice, maxPrice, page, pageSize);
        var result = await _getMenuItemsByPriceRangeHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
}
