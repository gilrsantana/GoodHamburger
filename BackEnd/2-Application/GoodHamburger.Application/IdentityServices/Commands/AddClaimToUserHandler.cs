using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IAddClaimToUserHandler
{
    Task<Result<AddClaimToUserResponse>> HandleAsync(AddClaimToUserCommand command, CancellationToken cancellationToken = default);
}

public class AddClaimToUserHandler : IAddClaimToUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AddClaimToUserHandler> _logger;

    public AddClaimToUserHandler(UserManager<ApplicationUser> userManager, ILogger<AddClaimToUserHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<AddClaimToUserResponse>> HandleAsync(AddClaimToUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return Result<AddClaimToUserResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {command.UserId} not found")
                );
            }

            var claim = new Claim(command.ClaimType, command.ClaimValue);
            var result = await _userManager.AddClaimAsync(user, claim);

            if (result.Succeeded)
            {
                _logger.LogInformation("Claim {ClaimType}:{ClaimValue} added to user {UserId} successfully", 
                    command.ClaimType, command.ClaimValue, command.UserId);
                
                return Result<AddClaimToUserResponse>.Success(new AddClaimToUserResponse(
                    command.UserId,
                    command.ClaimType,
                    command.ClaimValue,
                    true,
                    []
                ));
            }

            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("Claim addition failed: {Errors}", string.Join(", ", errors));

            return Result<AddClaimToUserResponse>.Failure(
                new Error("Claim.AdditionFailed", string.Join(", ", errors))
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding claim {ClaimType}:{ClaimValue} to user {UserId}", 
                command.ClaimType, command.ClaimValue, command.UserId);
            return Result<AddClaimToUserResponse>.Failure(
                new Error("Claim.AdditionError", "An error occurred while adding the claim to the user")
            );
        }
    }
}
