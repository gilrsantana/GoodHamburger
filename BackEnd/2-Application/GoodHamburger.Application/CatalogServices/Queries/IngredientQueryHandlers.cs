using GoodHamburger.Shared.Handlers;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.CatalogServices.Queries;

public interface IGetAllIngredientsHandler
{
    Task<Result<GetAllIngredientsResponse>> HandleAsync(GetAllIngredientsQuery query, CancellationToken cancellationToken = default);
}

public interface IGetIngredientByIdHandler
{
    Task<Result<GetIngredientByIdResponse>> HandleAsync(GetIngredientByIdQuery query, CancellationToken cancellationToken = default);
}

public interface IGetActiveIngredientsHandler
{
    Task<Result<GetActiveIngredientsResponse>> HandleAsync(GetActiveIngredientsQuery query, CancellationToken cancellationToken = default);
}

public interface ISearchIngredientsHandler
{
    Task<Result<SearchIngredientsResponse>> HandleAsync(SearchIngredientsQuery query, CancellationToken cancellationToken = default);
}

public interface IGetIngredientsByPriceRangeHandler
{
    Task<Result<GetIngredientsByPriceRangeResponse>> HandleAsync(GetIngredientsByPriceRangeQuery query, CancellationToken cancellationToken = default);
}

public class GetAllIngredientsHandler : IGetAllIngredientsHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<GetAllIngredientsHandler> _logger;

    public GetAllIngredientsHandler(IIngredientRepository ingredientRepository, ILogger<GetAllIngredientsHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<GetAllIngredientsResponse>> HandleAsync(GetAllIngredientsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredients = await _ingredientRepository.GetAllAsync(cancellationToken);
            
            var totalCount = ingredients.Count;
            var paginatedItems = ingredients
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(i => new IngredientDto(
                    i.Id,
                    i.Name,
                    i.Sku,
                    i.Active,
                    i.SalePrice,
                    i.ReferenceCostPrice,
                    i.CreatedAt,
                    i.UpdatedAt
                ))
                .ToList();

            return Result<GetAllIngredientsResponse>.Success(new GetAllIngredientsResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all ingredients");
            return Result<GetAllIngredientsResponse>.Failure(
                new Error("Ingredient.QueryError", "An error occurred while retrieving ingredients")
            );
        }
    }
}

public class GetIngredientByIdHandler : IGetIngredientByIdHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<GetIngredientByIdHandler> _logger;

    public GetIngredientByIdHandler(IIngredientRepository ingredientRepository, ILogger<GetIngredientByIdHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<GetIngredientByIdResponse>> HandleAsync(GetIngredientByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(query.Id, cancellationToken);
            
            if (ingredient == null)
            {
                return Result<GetIngredientByIdResponse>.Success(new GetIngredientByIdResponse(null));
            }

            var dto = new IngredientDto(
                ingredient.Id,
                ingredient.Name,
                ingredient.Sku,
                ingredient.Active,
                ingredient.SalePrice,
                ingredient.ReferenceCostPrice,
                ingredient.CreatedAt,
                ingredient.UpdatedAt
            );

            return Result<GetIngredientByIdResponse>.Success(new GetIngredientByIdResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting ingredient by ID {IngredientId}", query.Id);
            return Result<GetIngredientByIdResponse>.Failure(
                new Error("Ingredient.QueryError", "An error occurred while retrieving the ingredient")
            );
        }
    }
}

public class GetActiveIngredientsHandler : IGetActiveIngredientsHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<GetActiveIngredientsHandler> _logger;

    public GetActiveIngredientsHandler(IIngredientRepository ingredientRepository, ILogger<GetActiveIngredientsHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<GetActiveIngredientsResponse>> HandleAsync(GetActiveIngredientsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredients = await _ingredientRepository.GetActiveIngredientsAsync(cancellationToken);
            
            var totalCount = ingredients.Count;
            var paginatedItems = ingredients
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(i => new IngredientDto(
                    i.Id,
                    i.Name,
                    i.Sku,
                    i.Active,
                    i.SalePrice,
                    i.ReferenceCostPrice,
                    i.CreatedAt,
                    i.UpdatedAt
                ))
                .ToList();

            return Result<GetActiveIngredientsResponse>.Success(new GetActiveIngredientsResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting active ingredients");
            return Result<GetActiveIngredientsResponse>.Failure(
                new Error("Ingredient.QueryError", "An error occurred while retrieving ingredients")
            );
        }
    }
}

public class SearchIngredientsHandler : ISearchIngredientsHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<SearchIngredientsHandler> _logger;

    public SearchIngredientsHandler(IIngredientRepository ingredientRepository, ILogger<SearchIngredientsHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<SearchIngredientsResponse>> HandleAsync(SearchIngredientsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredients = await _ingredientRepository.SearchByNameAsync(query.SearchTerm, cancellationToken);
            
            var totalCount = ingredients.Count;
            var paginatedItems = ingredients
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(i => new IngredientDto(
                    i.Id,
                    i.Name,
                    i.Sku,
                    i.Active,
                    i.SalePrice,
                    i.ReferenceCostPrice,
                    i.CreatedAt,
                    i.UpdatedAt
                ))
                .ToList();

            return Result<SearchIngredientsResponse>.Success(new SearchIngredientsResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching ingredients with term {SearchTerm}", query.SearchTerm);
            return Result<SearchIngredientsResponse>.Failure(
                new Error("Ingredient.QueryError", "An error occurred while searching ingredients")
            );
        }
    }
}

public class GetIngredientsByPriceRangeHandler : IGetIngredientsByPriceRangeHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<GetIngredientsByPriceRangeHandler> _logger;

    public GetIngredientsByPriceRangeHandler(IIngredientRepository ingredientRepository, ILogger<GetIngredientsByPriceRangeHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<GetIngredientsByPriceRangeResponse>> HandleAsync(GetIngredientsByPriceRangeQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredients = await _ingredientRepository.GetIngredientsByPriceRangeAsync(query.MinPrice, query.MaxPrice, cancellationToken);
            
            var totalCount = ingredients.Count;
            var paginatedItems = ingredients
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(i => new IngredientDto(
                    i.Id,
                    i.Name,
                    i.Sku,
                    i.Active,
                    i.SalePrice,
                    i.ReferenceCostPrice,
                    i.CreatedAt,
                    i.UpdatedAt
                ))
                .ToList();

            return Result<GetIngredientsByPriceRangeResponse>.Success(new GetIngredientsByPriceRangeResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting ingredients by price range {MinPrice}-{MaxPrice}", query.MinPrice, query.MaxPrice);
            return Result<GetIngredientsByPriceRangeResponse>.Failure(
                new Error("Ingredient.QueryError", "An error occurred while retrieving ingredients")
            );
        }
    }
}
