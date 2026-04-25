using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IResetPasswordHandler
{
    Task<Result<ResetPasswordResponse>> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default);
}

public class ResetPasswordHandler : IResetPasswordHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResetPasswordHandler> _logger;

    public ResetPasswordHandler(UserManager<ApplicationUser> userManager, ILogger<ResetPasswordHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<ResetPasswordResponse>> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return Result<ResetPasswordResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {command.UserId} not found")
                );
            }

            var result = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successfully for user {UserId}", command.UserId);
                
                return Result<ResetPasswordResponse>.Success(new ResetPasswordResponse(
                    command.UserId,
                    true,
                    []
                ));
            }

            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("Password reset failed: {Errors}", string.Join(", ", errors));

            return Result<ResetPasswordResponse>.Failure(
                new Error("Password.ResetFailed", string.Join(", ", errors))
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while resetting password for user {UserId}", command.UserId);
            return Result<ResetPasswordResponse>.Failure(
                new Error("Password.ResetError", "An error occurred while resetting the password")
            );
        }
    }
}
