using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.CatalogServices.Commands;
using GoodHamburger.Application.CatalogServices.Queries;
using GoodHamburger.Domain.Catalog.Enums;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : BaseApiController
{
    private readonly ICreateCategoryHandler _createCategoryHandler;
    private readonly IUpdateCategoryHandler _updateCategoryHandler;
    private readonly IDeleteCategoryHandler _deleteCategoryHandler;
    private readonly IActivateCategoryHandler _activateCategoryHandler;
    private readonly IDeactivateCategoryHandler _deactivateCategoryHandler;
    private readonly IGetAllCategoriesHandler _getAllCategoriesHandler;
    private readonly IGetCategoryByIdHandler _getCategoryByIdHandler;
    private readonly IGetCategoryBySlugHandler _getCategoryBySlugHandler;
    private readonly IGetCategoriesByTypeHandler _getCategoriesByTypeHandler;
    private readonly IGetActiveCategoriesHandler _getActiveCategoriesHandler;

    public CategoriesController(
        ICreateCategoryHandler createCategoryHandler,
        IUpdateCategoryHandler updateCategoryHandler,
        IDeleteCategoryHandler deleteCategoryHandler,
        IActivateCategoryHandler activateCategoryHandler,
        IDeactivateCategoryHandler deactivateCategoryHandler,
        IGetAllCategoriesHandler getAllCategoriesHandler,
        IGetCategoryByIdHandler getCategoryByIdHandler,
        IGetCategoryBySlugHandler getCategoryBySlugHandler,
        IGetCategoriesByTypeHandler getCategoriesByTypeHandler,
        IGetActiveCategoriesHandler getActiveCategoriesHandler)
    {
        _createCategoryHandler = createCategoryHandler;
        _updateCategoryHandler = updateCategoryHandler;
        _deleteCategoryHandler = deleteCategoryHandler;
        _activateCategoryHandler = activateCategoryHandler;
        _deactivateCategoryHandler = deactivateCategoryHandler;
        _getAllCategoriesHandler = getAllCategoriesHandler;
        _getCategoryByIdHandler = getCategoryByIdHandler;
        _getCategoryBySlugHandler = getCategoryBySlugHandler;
        _getCategoriesByTypeHandler = getCategoriesByTypeHandler;
        _getActiveCategoriesHandler = getActiveCategoriesHandler;
    }

    [HttpPost]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(CreateCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await _createCategoryHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { CategoryId = id };
        var result = await _updateCategoryHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(DeleteCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await _deleteCategoryHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPost("{id}/activate")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(ActivateCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateCategoryCommand(id);
        var result = await _activateCategoryHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(DeactivateCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateCategoryCommand(id);
        var result = await _deactivateCategoryHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetAllCategoriesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCategoriesQuery(page, pageSize);
        var result = await _getAllCategoriesHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetCategoryByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await _getCategoryByIdHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetCategoryBySlugResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryBySlug(string slug, CancellationToken cancellationToken)
    {
        var query = new GetCategoryBySlugQuery(slug);
        var result = await _getCategoryBySlugHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("type/{type}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetCategoriesByTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategoriesByType(
        MenuCategoryType type,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoriesByTypeQuery(type, page, pageSize);
        var result = await _getCategoriesByTypeHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetActiveCategoriesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetActiveCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetActiveCategoriesQuery(page, pageSize);
        var result = await _getActiveCategoriesHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
}
