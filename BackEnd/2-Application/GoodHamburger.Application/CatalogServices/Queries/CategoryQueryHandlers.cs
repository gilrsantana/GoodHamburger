using GoodHamburger.Shared.Handlers;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.CatalogServices.Queries;

public interface IGetAllCategoriesHandler
{
    Task<Result<GetAllCategoriesResponse>> HandleAsync(GetAllCategoriesQuery query, CancellationToken cancellationToken = default);
}

public interface IGetCategoryByIdHandler
{
    Task<Result<GetCategoryByIdResponse>> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken = default);
}

public interface IGetCategoryBySlugHandler
{
    Task<Result<GetCategoryBySlugResponse>> HandleAsync(GetCategoryBySlugQuery query, CancellationToken cancellationToken = default);
}

public interface IGetCategoriesByTypeHandler
{
    Task<Result<GetCategoriesByTypeResponse>> HandleAsync(GetCategoriesByTypeQuery query, CancellationToken cancellationToken = default);
}

public interface IGetActiveCategoriesHandler
{
    Task<Result<GetActiveCategoriesResponse>> HandleAsync(GetActiveCategoriesQuery query, CancellationToken cancellationToken = default);
}

public class GetAllCategoriesHandler : IGetAllCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GetAllCategoriesHandler> _logger;

    public GetAllCategoriesHandler(ICategoryRepository categoryRepository, ILogger<GetAllCategoriesHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<GetAllCategoriesResponse>> HandleAsync(GetAllCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _categoryRepository.GetOrderedByDisplayAsync(cancellationToken);
            
            var totalCount = categories.Count;
            var paginatedItems = categories
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new CategoryDto(
                    c.Id,
                    c.Name,
                    c.Description,
                    c.Slug,
                    c.Type,
                    c.Active,
                    c.DisplayOrder,
                    c.ImageUrl,
                    c.CreatedAt,
                    c.UpdatedAt
                ))
                .ToList();

            return Result<GetAllCategoriesResponse>.Success(new GetAllCategoriesResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all categories");
            return Result<GetAllCategoriesResponse>.Failure(
                new Error("Category.QueryError", "An error occurred while retrieving categories")
            );
        }
    }
}

public class GetCategoryByIdHandler : IGetCategoryByIdHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GetCategoryByIdHandler> _logger;

    public GetCategoryByIdHandler(ICategoryRepository categoryRepository, ILogger<GetCategoryByIdHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<GetCategoryByIdResponse>> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(query.Id, cancellationToken);
            
            if (category == null)
            {
                return Result<GetCategoryByIdResponse>.Success(new GetCategoryByIdResponse(null));
            }

            var dto = new CategoryDto(
                category.Id,
                category.Name,
                category.Description,
                category.Slug,
                category.Type,
                category.Active,
                category.DisplayOrder,
                category.ImageUrl,
                category.CreatedAt,
                category.UpdatedAt
            );

            return Result<GetCategoryByIdResponse>.Success(new GetCategoryByIdResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting category by ID {CategoryId}", query.Id);
            return Result<GetCategoryByIdResponse>.Failure(
                new Error("Category.QueryError", "An error occurred while retrieving the category")
            );
        }
    }
}

public class GetCategoryBySlugHandler : IGetCategoryBySlugHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GetCategoryBySlugHandler> _logger;

    public GetCategoryBySlugHandler(ICategoryRepository categoryRepository, ILogger<GetCategoryBySlugHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<GetCategoryBySlugResponse>> HandleAsync(GetCategoryBySlugQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _categoryRepository.GetBySlugAsync(query.Slug, cancellationToken);
            
            if (category == null)
            {
                return Result<GetCategoryBySlugResponse>.Success(new GetCategoryBySlugResponse(null));
            }

            var dto = new CategoryDto(
                category.Id,
                category.Name,
                category.Description,
                category.Slug,
                category.Type,
                category.Active,
                category.DisplayOrder,
                category.ImageUrl,
                category.CreatedAt,
                category.UpdatedAt
            );

            return Result<GetCategoryBySlugResponse>.Success(new GetCategoryBySlugResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting category by slug {Slug}", query.Slug);
            return Result<GetCategoryBySlugResponse>.Failure(
                new Error("Category.QueryError", "An error occurred while retrieving the category")
            );
        }
    }
}

public class GetCategoriesByTypeHandler : IGetCategoriesByTypeHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GetCategoriesByTypeHandler> _logger;

    public GetCategoriesByTypeHandler(ICategoryRepository categoryRepository, ILogger<GetCategoriesByTypeHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<GetCategoriesByTypeResponse>> HandleAsync(GetCategoriesByTypeQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _categoryRepository.GetByTypeAsync(query.Type, cancellationToken);
            
            var totalCount = categories.Count;
            var paginatedItems = categories
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new CategoryDto(
                    c.Id,
                    c.Name,
                    c.Description,
                    c.Slug,
                    c.Type,
                    c.Active,
                    c.DisplayOrder,
                    c.ImageUrl,
                    c.CreatedAt,
                    c.UpdatedAt
                ))
                .ToList();

            return Result<GetCategoriesByTypeResponse>.Success(new GetCategoriesByTypeResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting categories by type {Type}", query.Type);
            return Result<GetCategoriesByTypeResponse>.Failure(
                new Error("Category.QueryError", "An error occurred while retrieving categories")
            );
        }
    }
}

public class GetActiveCategoriesHandler : IGetActiveCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GetActiveCategoriesHandler> _logger;

    public GetActiveCategoriesHandler(ICategoryRepository categoryRepository, ILogger<GetActiveCategoriesHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<GetActiveCategoriesResponse>> HandleAsync(GetActiveCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _categoryRepository.GetActiveCategoriesAsync(cancellationToken);
            
            var totalCount = categories.Count;
            var paginatedItems = categories
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new CategoryDto(
                    c.Id,
                    c.Name,
                    c.Description,
                    c.Slug,
                    c.Type,
                    c.Active,
                    c.DisplayOrder,
                    c.ImageUrl,
                    c.CreatedAt,
                    c.UpdatedAt
                ))
                .ToList();

            return Result<GetActiveCategoriesResponse>.Success(new GetActiveCategoriesResponse(
                paginatedItems,
                totalCount,
                query.Page,
                query.PageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting active categories");
            return Result<GetActiveCategoriesResponse>.Failure(
                new Error("Category.QueryError", "An error occurred while retrieving categories")
            );
        }
    }
}
