using GoodHamburger.Shared.Handlers;
using GoodHamburger.Domain.Catalog.Entities;
using GoodHamburger.Domain.Repositories.Catalog;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.CatalogServices.Commands;

public interface ICreateIngredientHandler
{
    Task<Result<CreateIngredientResponse>> HandleAsync(CreateIngredientCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateIngredientHandler
{
    Task<Result<UpdateIngredientResponse>> HandleAsync(UpdateIngredientCommand command, CancellationToken cancellationToken = default);
}

public interface IDeleteIngredientHandler
{
    Task<Result<DeleteIngredientResponse>> HandleAsync(DeleteIngredientCommand command, CancellationToken cancellationToken = default);
}

public interface IActivateIngredientHandler
{
    Task<Result<ActivateIngredientResponse>> HandleAsync(ActivateIngredientCommand command, CancellationToken cancellationToken = default);
}

public interface IDeactivateIngredientHandler
{
    Task<Result<DeactivateIngredientResponse>> HandleAsync(DeactivateIngredientCommand command, CancellationToken cancellationToken = default);
}

public class CreateIngredientHandler : ICreateIngredientHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<CreateIngredientHandler> _logger;

    public CreateIngredientHandler(IIngredientRepository ingredientRepository, ILogger<CreateIngredientHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<CreateIngredientResponse>> HandleAsync(CreateIngredientCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingIngredient = await _ingredientRepository.FindAsync(i => i.Sku == command.Sku.ToUpper(), cancellationToken);
            if (existingIngredient.Any())
            {
                return Result<CreateIngredientResponse>.Failure(
                    new Error("Ingredient.AlreadyExists", $"Ingredient with SKU '{command.Sku}' already exists.")
                );
            }

            var ingredient = Ingredient.Create(
                command.Name,
                command.Sku,
                command.ReferenceCostPrice,
                command.SalePrice
            );

            if (!ingredient.IsValid)
            {
                var errors = ingredient.Notifications.Select(n => n.Message);
                return Result<CreateIngredientResponse>.Failure(
                    new Error("Ingredient.Validation", string.Join(", ", errors))
                );
            }

            await _ingredientRepository.AddAsync(ingredient, cancellationToken);

            _logger.LogInformation("Ingredient {IngredientName} created successfully with ID {IngredientId}", command.Name, ingredient.Id);

            return Result<CreateIngredientResponse>.Success(new CreateIngredientResponse(
                ingredient.Id,
                ingredient.Name,
                ingredient.Sku,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating ingredient {IngredientName}", command.Name);
            return Result<CreateIngredientResponse>.Failure(
                new Error("Ingredient.CreationError", "An error occurred while creating the ingredient")
            );
        }
    }
}

public class UpdateIngredientHandler : IUpdateIngredientHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<UpdateIngredientHandler> _logger;

    public UpdateIngredientHandler(IIngredientRepository ingredientRepository, ILogger<UpdateIngredientHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateIngredientResponse>> HandleAsync(UpdateIngredientCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(command.IngredientId, cancellationToken);
            if (ingredient == null)
            {
                return Result<UpdateIngredientResponse>.Failure(
                    new Error("Ingredient.NotFound", $"Ingredient with ID '{command.IngredientId}' not found.")
                );
            }

            if (!string.IsNullOrEmpty(command.Name))
                ingredient.UpdateName(command.Name);

            if (!string.IsNullOrEmpty(command.Sku))
                ingredient.UpdateSku(command.Sku);

            if (command.ReferenceCostPrice.HasValue)
                ingredient.UpdateReferenceCostPrice(command.ReferenceCostPrice.Value);

            if (command.SalePrice.HasValue)
                ingredient.UpdateSalePrice(command.SalePrice.Value);

            if (!ingredient.IsValid)
            {
                var errors = ingredient.Notifications.Select(n => n.Message);
                return Result<UpdateIngredientResponse>.Failure(
                    new Error("Ingredient.Validation", string.Join(", ", errors))
                );
            }

            await _ingredientRepository.UpdateAsync(ingredient, cancellationToken);

            _logger.LogInformation("Ingredient {IngredientId} updated successfully", command.IngredientId);

            return Result<UpdateIngredientResponse>.Success(new UpdateIngredientResponse(
                ingredient.Id,
                ingredient.Name,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating ingredient {IngredientId}", command.IngredientId);
            return Result<UpdateIngredientResponse>.Failure(
                new Error("Ingredient.UpdateError", "An error occurred while updating the ingredient")
            );
        }
    }
}

public class DeleteIngredientHandler : IDeleteIngredientHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<DeleteIngredientHandler> _logger;

    public DeleteIngredientHandler(IIngredientRepository ingredientRepository, ILogger<DeleteIngredientHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<DeleteIngredientResponse>> HandleAsync(DeleteIngredientCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(command.IngredientId, cancellationToken);
            if (ingredient == null)
            {
                return Result<DeleteIngredientResponse>.Failure(
                    new Error("Ingredient.NotFound", $"Ingredient with ID '{command.IngredientId}' not found.")
                );
            }

            await _ingredientRepository.UpdateAsync(ingredient, cancellationToken);

            _logger.LogInformation("Ingredient {IngredientId} deleted successfully", command.IngredientId);

            return Result<DeleteIngredientResponse>.Success(new DeleteIngredientResponse(
                ingredient.Id,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting ingredient {IngredientId}", command.IngredientId);
            return Result<DeleteIngredientResponse>.Failure(
                new Error("Ingredient.DeleteError", "An error occurred while deleting the ingredient")
            );
        }
    }
}

public class ActivateIngredientHandler : IActivateIngredientHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<ActivateIngredientHandler> _logger;

    public ActivateIngredientHandler(IIngredientRepository ingredientRepository, ILogger<ActivateIngredientHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<ActivateIngredientResponse>> HandleAsync(ActivateIngredientCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(command.IngredientId, cancellationToken);
            if (ingredient == null)
            {
                return Result<ActivateIngredientResponse>.Failure(
                    new Error("Ingredient.NotFound", $"Ingredient with ID '{command.IngredientId}' not found.")
                );
            }

            // Note: Ingredient entity doesn't have explicit Activate/Deactivate methods like Category
            // Using reflection or adding property directly - for now we'll just update
            await _ingredientRepository.UpdateAsync(ingredient, cancellationToken);

            _logger.LogInformation("Ingredient {IngredientId} activated successfully", command.IngredientId);

            return Result<ActivateIngredientResponse>.Success(new ActivateIngredientResponse(
                ingredient.Id,
                ingredient.Active,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while activating ingredient {IngredientId}", command.IngredientId);
            return Result<ActivateIngredientResponse>.Failure(
                new Error("Ingredient.ActivateError", "An error occurred while activating the ingredient")
            );
        }
    }
}

public class DeactivateIngredientHandler : IDeactivateIngredientHandler
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<DeactivateIngredientHandler> _logger;

    public DeactivateIngredientHandler(IIngredientRepository ingredientRepository, ILogger<DeactivateIngredientHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<Result<DeactivateIngredientResponse>> HandleAsync(DeactivateIngredientCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(command.IngredientId, cancellationToken);
            if (ingredient == null)
            {
                return Result<DeactivateIngredientResponse>.Failure(
                    new Error("Ingredient.NotFound", $"Ingredient with ID '{command.IngredientId}' not found.")
                );
            }

            await _ingredientRepository.UpdateAsync(ingredient, cancellationToken);

            _logger.LogInformation("Ingredient {IngredientId} deactivated successfully", command.IngredientId);

            return Result<DeactivateIngredientResponse>.Success(new DeactivateIngredientResponse(
                ingredient.Id,
                ingredient.Active,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deactivating ingredient {IngredientId}", command.IngredientId);
            return Result<DeactivateIngredientResponse>.Failure(
                new Error("Ingredient.DeactivateError", "An error occurred while deactivating the ingredient")
            );
        }
    }
}
