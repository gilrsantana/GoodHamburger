using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IGeneratePasswordResetTokenHandler
{
    Task<Result<GeneratePasswordResetTokenResponse>> HandleAsync(GeneratePasswordResetTokenCommand command, CancellationToken cancellationToken = default);
}

public class GeneratePasswordResetTokenHandler : IGeneratePasswordResetTokenHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GeneratePasswordResetTokenHandler> _logger;

    public GeneratePasswordResetTokenHandler(UserManager<ApplicationUser> userManager, ILogger<GeneratePasswordResetTokenHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<GeneratePasswordResetTokenResponse>> HandleAsync(GeneratePasswordResetTokenCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return Result<GeneratePasswordResetTokenResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {command.UserId} not found")
                );
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Password reset token generated for user {UserId}", command.UserId);
                
                return Result<GeneratePasswordResetTokenResponse>.Success(new GeneratePasswordResetTokenResponse(
                    command.UserId,
                    token,
                    true,
                    []
                ));
            }

            _logger.LogWarning("Failed to generate password reset token for user {UserId}", command.UserId);

            return Result<GeneratePasswordResetTokenResponse>.Failure(
                new Error("Token.GenerationFailed", "Failed to generate password reset token")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating password reset token for user {UserId}", command.UserId);
            return Result<GeneratePasswordResetTokenResponse>.Failure(
                new Error("Token.GenerationError", "An error occurred while generating the password reset token")
            );
        }
    }
}
