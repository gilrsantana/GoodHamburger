using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IAssignRoleToUserHandler
{
    Task<Result<AssignRoleToUserResponse>> HandleAsync(AssignRoleToUserCommand command, CancellationToken cancellationToken = default);
}

public class AssignRoleToUserHandler : IAssignRoleToUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<AssignRoleToUserHandler> _logger;

    public AssignRoleToUserHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<AssignRoleToUserHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result<AssignRoleToUserResponse>> HandleAsync(AssignRoleToUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return Result<AssignRoleToUserResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {command.UserId} not found")
                );
            }

            var role = await _roleManager.FindByNameAsync(command.RoleName);
            if (role == null)
            {
                return Result<AssignRoleToUserResponse>.Failure(
                    new Error("Role.NotFound", $"Role '{command.RoleName}' not found")
                );
            }

            var result = await _userManager.AddToRoleAsync(user, command.RoleName);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} assigned to role {RoleName} successfully", command.UserId, command.RoleName);
                
                return Result<AssignRoleToUserResponse>.Success(new AssignRoleToUserResponse(
                    command.UserId,
                    command.RoleName,
                    true,
                    []
                ));
            }

            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("Role assignment failed: {Errors}", string.Join(", ", errors));

            return Result<AssignRoleToUserResponse>.Failure(
                new Error("Role.AssignmentFailed", string.Join(", ", errors))
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while assigning role {RoleName} to user {UserId}", command.RoleName, command.UserId);
            return Result<AssignRoleToUserResponse>.Failure(
                new Error("Role.AssignmentError", "An error occurred while assigning the role to the user")
            );
        }
    }
}
