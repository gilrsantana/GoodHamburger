using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IRemoveClaimFromUserHandler
{
    Task<Result<RemoveClaimFromUserResponse>> HandleAsync(RemoveClaimFromUserCommand command, CancellationToken cancellationToken = default);
}

public class RemoveClaimFromUserHandler : IRemoveClaimFromUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RemoveClaimFromUserHandler> _logger;

    public RemoveClaimFromUserHandler(UserManager<ApplicationUser> userManager, ILogger<RemoveClaimFromUserHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<RemoveClaimFromUserResponse>> HandleAsync(RemoveClaimFromUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return Result<RemoveClaimFromUserResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {command.UserId} not found")
                );
            }

            var claim = new Claim(command.ClaimType, command.ClaimValue);
            var result = await _userManager.RemoveClaimAsync(user, claim);

            if (result.Succeeded)
            {
                _logger.LogInformation("Claim {ClaimType}:{ClaimValue} removed from user {UserId} successfully", 
                    command.ClaimType, command.ClaimValue, command.UserId);
                
                return Result<RemoveClaimFromUserResponse>.Success(new RemoveClaimFromUserResponse(
                    command.UserId,
                    command.ClaimType,
                    command.ClaimValue,
                    true,
                    []
                ));
            }

            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("Claim removal failed: {Errors}", string.Join(", ", errors));

            return Result<RemoveClaimFromUserResponse>.Failure(
                new Error("Claim.RemovalFailed", string.Join(", ", errors))
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing claim {ClaimType}:{ClaimValue} from user {UserId}", 
                command.ClaimType, command.ClaimValue, command.UserId);
            return Result<RemoveClaimFromUserResponse>.Failure(
                new Error("Claim.RemovalError", "An error occurred while removing the claim from the user")
            );
        }
    }
}
