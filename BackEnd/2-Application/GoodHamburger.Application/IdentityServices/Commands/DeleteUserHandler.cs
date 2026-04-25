using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IDeleteUserHandler
{
    Task<Result<DeleteUserResponse>> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default);
}

public class DeleteUserHandler : IDeleteUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DeleteUserHandler> _logger;

    public DeleteUserHandler(UserManager<ApplicationUser> userManager, ILogger<DeleteUserHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<DeleteUserResponse>> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return Result<DeleteUserResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {command.UserId} not found")
                );
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} deleted successfully", command.UserId);
                return Result<DeleteUserResponse>.Success(new DeleteUserResponse(
                    command.UserId,
                    true,
                    []
                ));
            }

            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("User deletion failed: {Errors}", string.Join(", ", errors));

            return Result<DeleteUserResponse>.Failure(
                new Error("User.DeletionFailed", string.Join(", ", errors))
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user {UserId}", command.UserId);
            return Result<DeleteUserResponse>.Failure(
                new Error("User.DeletionError", "An error occurred while deleting the user")
            );
        }
    }
}
