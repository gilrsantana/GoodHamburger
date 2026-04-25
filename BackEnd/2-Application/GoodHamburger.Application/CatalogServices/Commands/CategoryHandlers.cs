using GoodHamburger.Shared.Handlers;
using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.CatalogServices.Commands;

public interface ICreateCategoryHandler
{
    Task<Result<CreateCategoryResponse>> HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateCategoryHandler
{
    Task<Result<UpdateCategoryResponse>> HandleAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default);
}

public interface IDeleteCategoryHandler
{
    Task<Result<DeleteCategoryResponse>> HandleAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default);
}

public interface IActivateCategoryHandler
{
    Task<Result<ActivateCategoryResponse>> HandleAsync(ActivateCategoryCommand command, CancellationToken cancellationToken = default);
}

public interface IDeactivateCategoryHandler
{
    Task<Result<DeactivateCategoryResponse>> HandleAsync(DeactivateCategoryCommand command, CancellationToken cancellationToken = default);
}

public class CreateCategoryHandler : ICreateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CreateCategoryHandler> _logger;

    public CreateCategoryHandler(ICategoryRepository categoryRepository, ILogger<CreateCategoryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<CreateCategoryResponse>> HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingCategory = await _categoryRepository.GetBySlugAsync(command.Slug, cancellationToken);
            if (existingCategory != null)
            {
                return Result<CreateCategoryResponse>.Failure(
                    new Error("Category.AlreadyExists", $"Category with slug '{command.Slug}' already exists.")
                );
            }

            var category = Category.Create(
                command.Name,
                command.Description,
                command.Slug,
                command.Type,
                command.DisplayOrder,
                command.ImageUrl
            );

            if (!category.IsValid)
            {
                var errors = category.Notifications.Select(n => n.Message);
                return Result<CreateCategoryResponse>.Failure(
                    new Error("Category.Validation", string.Join(", ", errors))
                );
            }

            await _categoryRepository.AddAsync(category, cancellationToken);

            _logger.LogInformation("Category {CategoryName} created successfully with ID {CategoryId}", command.Name, category.Id);

            return Result<CreateCategoryResponse>.Success(new CreateCategoryResponse(
                category.Id,
                category.Name,
                category.Slug,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating category {CategoryName}", command.Name);
            return Result<CreateCategoryResponse>.Failure(
                new Error("Category.CreationError", "An error occurred while creating the category")
            );
        }
    }
}

public class UpdateCategoryHandler : IUpdateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<UpdateCategoryHandler> _logger;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, ILogger<UpdateCategoryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateCategoryResponse>> HandleAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<UpdateCategoryResponse>.Failure(
                    new Error("Category.NotFound", $"Category with ID '{command.CategoryId}' not found.")
                );
            }

            if (!string.IsNullOrEmpty(command.Name))
                category.UpdateName(command.Name);

            if (!string.IsNullOrEmpty(command.Description))
                category.UpdateDescription(command.Description);

            if (!string.IsNullOrEmpty(command.Slug))
                category.UpdateSlug(command.Slug);

            if (command.DisplayOrder.HasValue)
                category.UpdateDisplayOrder(command.DisplayOrder.Value);

            if (command.ImageUrl != null)
                category.UpdateImage(command.ImageUrl);

            if (!category.IsValid)
            {
                var errors = category.Notifications.Select(n => n.Message);
                return Result<UpdateCategoryResponse>.Failure(
                    new Error("Category.Validation", string.Join(", ", errors))
                );
            }

            await _categoryRepository.UpdateAsync(category, cancellationToken);

            _logger.LogInformation("Category {CategoryId} updated successfully", command.CategoryId);

            return Result<UpdateCategoryResponse>.Success(new UpdateCategoryResponse(
                category.Id,
                category.Name,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating category {CategoryId}", command.CategoryId);
            return Result<UpdateCategoryResponse>.Failure(
                new Error("Category.UpdateError", "An error occurred while updating the category")
            );
        }
    }
}

public class DeleteCategoryHandler : IDeleteCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<DeleteCategoryHandler> _logger;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, ILogger<DeleteCategoryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<DeleteCategoryResponse>> HandleAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<DeleteCategoryResponse>.Failure(
                    new Error("Category.NotFound", $"Category with ID '{command.CategoryId}' not found.")
                );
            }

            // Soft delete by deactivating
            category.Deactivate();
            await _categoryRepository.UpdateAsync(category, cancellationToken);

            _logger.LogInformation("Category {CategoryId} deleted (deactivated) successfully", command.CategoryId);

            return Result<DeleteCategoryResponse>.Success(new DeleteCategoryResponse(
                category.Id,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting category {CategoryId}", command.CategoryId);
            return Result<DeleteCategoryResponse>.Failure(
                new Error("Category.DeleteError", "An error occurred while deleting the category")
            );
        }
    }
}

public class ActivateCategoryHandler : IActivateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<ActivateCategoryHandler> _logger;

    public ActivateCategoryHandler(ICategoryRepository categoryRepository, ILogger<ActivateCategoryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<ActivateCategoryResponse>> HandleAsync(ActivateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<ActivateCategoryResponse>.Failure(
                    new Error("Category.NotFound", $"Category with ID '{command.CategoryId}' not found.")
                );
            }

            category.Activate();
            await _categoryRepository.UpdateAsync(category, cancellationToken);

            _logger.LogInformation("Category {CategoryId} activated successfully", command.CategoryId);

            return Result<ActivateCategoryResponse>.Success(new ActivateCategoryResponse(
                category.Id,
                category.Active,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while activating category {CategoryId}", command.CategoryId);
            return Result<ActivateCategoryResponse>.Failure(
                new Error("Category.ActivateError", "An error occurred while activating the category")
            );
        }
    }
}

public class DeactivateCategoryHandler : IDeactivateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<DeactivateCategoryHandler> _logger;

    public DeactivateCategoryHandler(ICategoryRepository categoryRepository, ILogger<DeactivateCategoryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<DeactivateCategoryResponse>> HandleAsync(DeactivateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<DeactivateCategoryResponse>.Failure(
                    new Error("Category.NotFound", $"Category with ID '{command.CategoryId}' not found.")
                );
            }

            category.Deactivate();
            await _categoryRepository.UpdateAsync(category, cancellationToken);

            _logger.LogInformation("Category {CategoryId} deactivated successfully", command.CategoryId);

            return Result<DeactivateCategoryResponse>.Success(new DeactivateCategoryResponse(
                category.Id,
                category.Active,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deactivating category {CategoryId}", command.CategoryId);
            return Result<DeactivateCategoryResponse>.Failure(
                new Error("Category.DeactivateError", "An error occurred while deactivating the category")
            );
        }
    }
}
