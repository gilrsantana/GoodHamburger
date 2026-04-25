using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IGenerateEmailConfirmationTokenHandler
{
    Task<Result<GenerateEmailConfirmationTokenResponse>> HandleAsync(GenerateEmailConfirmationTokenCommand command, CancellationToken cancellationToken = default);
}

public class GenerateEmailConfirmationTokenHandler : IGenerateEmailConfirmationTokenHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GenerateEmailConfirmationTokenHandler> _logger;

    public GenerateEmailConfirmationTokenHandler(UserManager<ApplicationUser> userManager, ILogger<GenerateEmailConfirmationTokenHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<GenerateEmailConfirmationTokenResponse>> HandleAsync(GenerateEmailConfirmationTokenCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return Result<GenerateEmailConfirmationTokenResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {command.UserId} not found")
                );
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Email confirmation token generated for user {UserId}", command.UserId);
                
                return Result<GenerateEmailConfirmationTokenResponse>.Success(new GenerateEmailConfirmationTokenResponse(
                    command.UserId,
                    token,
                    true,
                    []
                ));
            }

            _logger.LogWarning("Failed to generate email confirmation token for user {UserId}", command.UserId);

            return Result<GenerateEmailConfirmationTokenResponse>.Failure(
                new Error("Token.GenerationFailed", "Failed to generate email confirmation token")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating email confirmation token for user {UserId}", command.UserId);
            return Result<GenerateEmailConfirmationTokenResponse>.Failure(
                new Error("Token.GenerationError", "An error occurred while generating the email confirmation token")
            );
        }
    }
}
