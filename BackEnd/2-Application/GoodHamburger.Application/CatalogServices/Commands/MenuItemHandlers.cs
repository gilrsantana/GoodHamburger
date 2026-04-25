using GoodHamburger.Shared.Handlers;
using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.CatalogServices.Commands;

public interface ICreateMenuItemHandler
{
    Task<Result<CreateMenuItemResponse>> HandleAsync(CreateMenuItemCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateMenuItemHandler
{
    Task<Result<UpdateMenuItemResponse>> HandleAsync(UpdateMenuItemCommand command, CancellationToken cancellationToken = default);
}

public interface IDeleteMenuItemHandler
{
    Task<Result<DeleteMenuItemResponse>> HandleAsync(DeleteMenuItemCommand command, CancellationToken cancellationToken = default);
}

public interface IActivateMenuItemHandler
{
    Task<Result<ActivateMenuItemResponse>> HandleAsync(ActivateMenuItemCommand command, CancellationToken cancellationToken = default);
}

public interface IDeactivateMenuItemHandler
{
    Task<Result<DeactivateMenuItemResponse>> HandleAsync(DeactivateMenuItemCommand command, CancellationToken cancellationToken = default);
}

public interface ISetMenuItemAvailabilityHandler
{
    Task<Result<SetMenuItemAvailabilityResponse>> HandleAsync(SetMenuItemAvailabilityCommand command, CancellationToken cancellationToken = default);
}

public interface IAddIngredientToMenuItemHandler
{
    Task<Result<AddIngredientToMenuItemResponse>> HandleAsync(AddIngredientToMenuItemCommand command, CancellationToken cancellationToken = default);
}

public interface IRemoveIngredientFromMenuItemHandler
{
    Task<Result<RemoveIngredientFromMenuItemResponse>> HandleAsync(RemoveIngredientFromMenuItemCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateIngredientRemovabilityHandler
{
    Task<Result<UpdateIngredientRemovabilityResponse>> HandleAsync(UpdateIngredientRemovabilityCommand command, CancellationToken cancellationToken = default);
}

public class CreateMenuItemHandler : ICreateMenuItemHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CreateMenuItemHandler> _logger;

    public CreateMenuItemHandler(
        IMenuItemRepository menuItemRepository,
        ICategoryRepository categoryRepository,
        ILogger<CreateMenuItemHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<CreateMenuItemResponse>> HandleAsync(CreateMenuItemCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<CreateMenuItemResponse>.Failure(
                    new Error("MenuItem.CategoryNotFound", $"Category with ID '{command.CategoryId}' not found.")
                );
            }

            var existingBySku = await _menuItemRepository.GetBySkuAsync(command.Sku, cancellationToken);
            if (existingBySku != null)
            {
                return Result<CreateMenuItemResponse>.Failure(
                    new Error("MenuItem.AlreadyExists", $"Menu item with SKU '{command.Sku}' already exists.")
                );
            }

            var existingBySlug = await _menuItemRepository.GetBySlugAsync(command.Slug, cancellationToken);
            if (existingBySlug != null)
            {
                return Result<CreateMenuItemResponse>.Failure(
                    new Error("MenuItem.AlreadyExists", $"Menu item with slug '{command.Slug}' already exists.")
                );
            }

            var menuItem = MenuItem.Create(
                command.Name,
                command.Description,
                command.Slug,
                command.Sku,
                command.Price,
                command.CategoryId,
                command.Calories,
                command.ImageUrl
            );

            if (!menuItem.IsValid)
            {
                var errors = menuItem.Notifications.Select(n => n.Message);
                return Result<CreateMenuItemResponse>.Failure(
                    new Error("MenuItem.Validation", string.Join(", ", errors))
                );
            }

            await _menuItemRepository.AddAsync(menuItem, cancellationToken);

            _logger.LogInformation("Menu item {MenuItemName} created successfully with ID {MenuItemId}", command.Name, menuItem.Id);

            return Result<CreateMenuItemResponse>.Success(new CreateMenuItemResponse(
                menuItem.Id,
                menuItem.Name,
                menuItem.Sku,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating menu item {MenuItemName}", command.Name);
            return Result<CreateMenuItemResponse>.Failure(
                new Error("MenuItem.CreationError", "An error occurred while creating the menu item")
            );
        }
    }
}

public class UpdateMenuItemHandler : IUpdateMenuItemHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<UpdateMenuItemHandler> _logger;

    public UpdateMenuItemHandler(
        IMenuItemRepository menuItemRepository,
        ICategoryRepository categoryRepository,
        ILogger<UpdateMenuItemHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateMenuItemResponse>> HandleAsync(UpdateMenuItemCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
            if (menuItem == null)
            {
                return Result<UpdateMenuItemResponse>.Failure(
                    new Error("MenuItem.NotFound", $"Menu item with ID '{command.MenuItemId}' not found.")
                );
            }

            if (!string.IsNullOrEmpty(command.Name))
                menuItem.UpdateName(command.Name);

            if (!string.IsNullOrEmpty(command.Description))
                menuItem.UpdateDescription(command.Description);

            if (!string.IsNullOrEmpty(command.Slug))
                menuItem.UpdateSlug(command.Slug);

            if (!string.IsNullOrEmpty(command.Sku))
                menuItem.UpdateSku(command.Sku);

            if (command.Price.HasValue)
                menuItem.UpdatePrice(command.Price.Value);

            if (command.Calories.HasValue)
                menuItem.UpdateCalories(command.Calories.Value);

            if (command.ImageUrl != null)
                menuItem.UpdateImage(command.ImageUrl);

            if (command.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);
                if (category == null)
                {
                    return Result<UpdateMenuItemResponse>.Failure(
                        new Error("MenuItem.CategoryNotFound", $"Category with ID '{command.CategoryId}' not found.")
                    );
                }
                menuItem.UpdateCategoryId(command.CategoryId.Value);
            }

            if (!menuItem.IsValid)
            {
                var errors = menuItem.Notifications.Select(n => n.Message);
                return Result<UpdateMenuItemResponse>.Failure(
                    new Error("MenuItem.Validation", string.Join(", ", errors))
                );
            }

            await _menuItemRepository.UpdateAsync(menuItem, cancellationToken);

            _logger.LogInformation("Menu item {MenuItemId} updated successfully", command.MenuItemId);

            return Result<UpdateMenuItemResponse>.Success(new UpdateMenuItemResponse(
                menuItem.Id,
                menuItem.Name,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating menu item {MenuItemId}", command.MenuItemId);
            return Result<UpdateMenuItemResponse>.Failure(
                new Error("MenuItem.UpdateError", "An error occurred while updating the menu item")
            );
        }
    }
}

public class DeleteMenuItemHandler : IDeleteMenuItemHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<DeleteMenuItemHandler> _logger;

    public DeleteMenuItemHandler(IMenuItemRepository menuItemRepository, ILogger<DeleteMenuItemHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<DeleteMenuItemResponse>> HandleAsync(DeleteMenuItemCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
            if (menuItem == null)
            {
                return Result<DeleteMenuItemResponse>.Failure(
                    new Error("MenuItem.NotFound", $"Menu item with ID '{command.MenuItemId}' not found.")
                );
            }

            menuItem.Deactivate();
            await _menuItemRepository.UpdateAsync(menuItem, cancellationToken);

            _logger.LogInformation("Menu item {MenuItemId} deleted (deactivated) successfully", command.MenuItemId);

            return Result<DeleteMenuItemResponse>.Success(new DeleteMenuItemResponse(
                menuItem.Id,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting menu item {MenuItemId}", command.MenuItemId);
            return Result<DeleteMenuItemResponse>.Failure(
                new Error("MenuItem.DeleteError", "An error occurred while deleting the menu item")
            );
        }
    }
}

public class ActivateMenuItemHandler : IActivateMenuItemHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<ActivateMenuItemHandler> _logger;

    public ActivateMenuItemHandler(IMenuItemRepository menuItemRepository, ILogger<ActivateMenuItemHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<ActivateMenuItemResponse>> HandleAsync(ActivateMenuItemCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
            if (menuItem == null)
            {
                return Result<ActivateMenuItemResponse>.Failure(
                    new Error("MenuItem.NotFound", $"Menu item with ID '{command.MenuItemId}' not found.")
                );
            }

            menuItem.Activate();
            await _menuItemRepository.UpdateAsync(menuItem, cancellationToken);

            _logger.LogInformation("Menu item {MenuItemId} activated successfully", command.MenuItemId);

            return Result<ActivateMenuItemResponse>.Success(new ActivateMenuItemResponse(
                menuItem.Id,
                menuItem.Active,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while activating menu item {MenuItemId}", command.MenuItemId);
            return Result<ActivateMenuItemResponse>.Failure(
                new Error("MenuItem.ActivateError", "An error occurred while activating the menu item")
            );
        }
    }
}

public class DeactivateMenuItemHandler : IDeactivateMenuItemHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<DeactivateMenuItemHandler> _logger;

    public DeactivateMenuItemHandler(IMenuItemRepository menuItemRepository, ILogger<DeactivateMenuItemHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<DeactivateMenuItemResponse>> HandleAsync(DeactivateMenuItemCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
            if (menuItem == null)
            {
                return Result<DeactivateMenuItemResponse>.Failure(
                    new Error("MenuItem.NotFound", $"Menu item with ID '{command.MenuItemId}' not found.")
                );
            }

            menuItem.Deactivate();
            await _menuItemRepository.UpdateAsync(menuItem, cancellationToken);

            _logger.LogInformation("Menu item {MenuItemId} deactivated successfully", command.MenuItemId);

            return Result<DeactivateMenuItemResponse>.Success(new DeactivateMenuItemResponse(
                menuItem.Id,
                menuItem.Active,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deactivating menu item {MenuItemId}", command.MenuItemId);
            return Result<DeactivateMenuItemResponse>.Failure(
                new Error("MenuItem.DeactivateError", "An error occurred while deactivating the menu item")
            );
        }
    }
}

public class SetMenuItemAvailabilityHandler : ISetMenuItemAvailabilityHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<SetMenuItemAvailabilityHandler> _logger;

    public SetMenuItemAvailabilityHandler(IMenuItemRepository menuItemRepository, ILogger<SetMenuItemAvailabilityHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<SetMenuItemAvailabilityResponse>> HandleAsync(SetMenuItemAvailabilityCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
            if (menuItem == null)
            {
                return Result<SetMenuItemAvailabilityResponse>.Failure(
                    new Error("MenuItem.NotFound", $"Menu item with ID '{command.MenuItemId}' not found.")
                );
            }

            if (command.IsAvailable)
                menuItem.SetAvailable();
            else
                menuItem.SetUnavailable();

            await _menuItemRepository.UpdateAsync(menuItem, cancellationToken);

            _logger.LogInformation("Menu item {MenuItemId} availability set to {IsAvailable}", command.MenuItemId, command.IsAvailable);

            return Result<SetMenuItemAvailabilityResponse>.Success(new SetMenuItemAvailabilityResponse(
                menuItem.Id,
                menuItem.IsAvailable,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting menu item {MenuItemId} availability", command.MenuItemId);
            return Result<SetMenuItemAvailabilityResponse>.Failure(
                new Error("MenuItem.AvailabilityError", "An error occurred while setting menu item availability")
            );
        }
    }
}

public class AddIngredientToMenuItemHandler : IAddIngredientToMenuItemHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<AddIngredientToMenuItemHandler> _logger;

    public AddIngredientToMenuItemHandler(
        IMenuItemRepository menuItemRepository,
        IIngredientRepository ingredientRepository,
        ILogger<AddIngredientToMenuItemHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<AddIngredientToMenuItemResponse>> HandleAsync(AddIngredientToMenuItemCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
            if (menuItem == null)
            {
                return Result<AddIngredientToMenuItemResponse>.Failure(
                    new Error("MenuItem.NotFound", $"Menu item with ID '{command.MenuItemId}' not found.")
                );
            }

            var ingredient = await _ingredientRepository.GetByIdAsync(command.IngredientId, cancellationToken);
            if (ingredient == null)
            {
                return Result<AddIngredientToMenuItemResponse>.Failure(
                    new Error("Ingredient.NotFound", $"Ingredient with ID '{command.IngredientId}' not found.")
                );
            }

            menuItem.AddIngredient(command.IngredientId, command.IsRemovable);

            if (!menuItem.IsValid)
            {
                var errors = menuItem.Notifications.Select(n => n.Message);
                return Result<AddIngredientToMenuItemResponse>.Failure(
                    new Error("MenuItem.Validation", string.Join(", ", errors))
                );
            }

            await _menuItemRepository.UpdateAsync(menuItem, cancellationToken);

            _logger.LogInformation("Ingredient {IngredientId} added to menu item {MenuItemId}", command.IngredientId, command.MenuItemId);

            return Result<AddIngredientToMenuItemResponse>.Success(new AddIngredientToMenuItemResponse(
                menuItem.Id,
                command.IngredientId,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding ingredient to menu item");
            return Result<AddIngredientToMenuItemResponse>.Failure(
                new Error("MenuItem.AddIngredientError", "An error occurred while adding the ingredient")
            );
        }
    }
}

public class RemoveIngredientFromMenuItemHandler : IRemoveIngredientFromMenuItemHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<RemoveIngredientFromMenuItemHandler> _logger;

    public RemoveIngredientFromMenuItemHandler(IMenuItemRepository menuItemRepository, ILogger<RemoveIngredientFromMenuItemHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<RemoveIngredientFromMenuItemResponse>> HandleAsync(RemoveIngredientFromMenuItemCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
            if (menuItem == null)
            {
                return Result<RemoveIngredientFromMenuItemResponse>.Failure(
                    new Error("MenuItem.NotFound", $"Menu item with ID '{command.MenuItemId}' not found.")
                );
            }

            menuItem.RemoveIngredient(command.IngredientId);
            await _menuItemRepository.UpdateAsync(menuItem, cancellationToken);

            _logger.LogInformation("Ingredient {IngredientId} removed from menu item {MenuItemId}", command.IngredientId, command.MenuItemId);

            return Result<RemoveIngredientFromMenuItemResponse>.Success(new RemoveIngredientFromMenuItemResponse(
                menuItem.Id,
                command.IngredientId,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing ingredient from menu item");
            return Result<RemoveIngredientFromMenuItemResponse>.Failure(
                new Error("MenuItem.RemoveIngredientError", "An error occurred while removing the ingredient")
            );
        }
    }
}

public class UpdateIngredientRemovabilityHandler : IUpdateIngredientRemovabilityHandler
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<UpdateIngredientRemovabilityHandler> _logger;

    public UpdateIngredientRemovabilityHandler(IMenuItemRepository menuItemRepository, ILogger<UpdateIngredientRemovabilityHandler> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateIngredientRemovabilityResponse>> HandleAsync(UpdateIngredientRemovabilityCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
            if (menuItem == null)
            {
                return Result<UpdateIngredientRemovabilityResponse>.Failure(
                    new Error("MenuItem.NotFound", $"Menu item with ID '{command.MenuItemId}' not found.")
                );
            }

            menuItem.UpdateIngredientIsRemovable(command.IngredientId, command.IsRemovable);

            if (!menuItem.IsValid)
            {
                var errors = menuItem.Notifications.Select(n => n.Message);
                return Result<UpdateIngredientRemovabilityResponse>.Failure(
                    new Error("MenuItem.Validation", string.Join(", ", errors))
                );
            }

            await _menuItemRepository.UpdateAsync(menuItem, cancellationToken);

            _logger.LogInformation("Ingredient {IngredientId} removability updated to {IsRemovable} for menu item {MenuItemId}", 
                command.IngredientId, command.IsRemovable, command.MenuItemId);

            return Result<UpdateIngredientRemovabilityResponse>.Success(new UpdateIngredientRemovabilityResponse(
                menuItem.Id,
                command.IngredientId,
                command.IsRemovable,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating ingredient removability");
            return Result<UpdateIngredientRemovabilityResponse>.Failure(
                new Error("MenuItem.UpdateRemovabilityError", "An error occurred while updating ingredient removability")
            );
        }
    }
}
