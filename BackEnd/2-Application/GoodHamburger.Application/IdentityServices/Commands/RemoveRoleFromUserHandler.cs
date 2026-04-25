using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IRemoveRoleFromUserHandler
{
    Task<Result<RemoveRoleFromUserResponse>> HandleAsync(RemoveRoleFromUserCommand command, CancellationToken cancellationToken = default);
}

public class RemoveRoleFromUserHandler : IRemoveRoleFromUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<RemoveRoleFromUserHandler> _logger;

    public RemoveRoleFromUserHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<RemoveRoleFromUserHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result<RemoveRoleFromUserResponse>> HandleAsync(RemoveRoleFromUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return Result<RemoveRoleFromUserResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {command.UserId} not found")
                );
            }

            var role = await _roleManager.FindByNameAsync(command.RoleName);
            if (role == null)
            {
                return Result<RemoveRoleFromUserResponse>.Failure(
                    new Error("Role.NotFound", $"Role '{command.RoleName}' not found")
                );
            }

            var result = await _userManager.RemoveFromRoleAsync(user, command.RoleName);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} removed from role {RoleName} successfully", command.UserId, command.RoleName);
                
                return Result<RemoveRoleFromUserResponse>.Success(new RemoveRoleFromUserResponse(
                    command.UserId,
                    command.RoleName,
                    true,
                    []
                ));
            }

            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("Role removal failed: {Errors}", string.Join(", ", errors));

            return Result<RemoveRoleFromUserResponse>.Failure(
                new Error("Role.RemovalFailed", string.Join(", ", errors))
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing role {RoleName} from user {UserId}", command.RoleName, command.UserId);
            return Result<RemoveRoleFromUserResponse>.Failure(
                new Error("Role.RemovalError", "An error occurred while removing the role from the user")
            );
        }
    }
}
