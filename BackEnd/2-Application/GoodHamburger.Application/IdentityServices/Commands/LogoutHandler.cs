using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface ILogoutHandler
{
    Task<Result<LogoutResponse>> HandleAsync(LogoutCommand command, CancellationToken cancellationToken = default);
}

public class LogoutHandler : ILogoutHandler
{
    private readonly IdentityDbContext _dbContext;
    private readonly ILogger<LogoutHandler> _logger;

    public LogoutHandler(
        IdentityDbContext dbContext,
        ILogger<LogoutHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<LogoutResponse>> HandleAsync(LogoutCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(command.UserId, out var userId))
            {
                _logger.LogWarning("Invalid user ID provided for logout: {UserId}", command.UserId);
                return Result<LogoutResponse>.Failure(
                    new Error("Logout.InvalidUserId", "Invalid user ID provided")
                );
            }

            // Find all active refresh tokens for the user
            var refreshTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.Expires >= DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            if (refreshTokens.Any())
            {
                // Revoke all active refresh tokens for the user
                foreach (var token in refreshTokens)
                {
                    token.IsRevoked = true;
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Revoked {Count} refresh tokens for user {UserId}", refreshTokens.Count, userId);
            }
            else
            {
                _logger.LogInformation("No active refresh tokens found for user {UserId}", userId);
            }

            return Result<LogoutResponse>.Success(new LogoutResponse(true, []));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during logout for user {UserId}", command.UserId);
            return Result<LogoutResponse>.Failure(
                new Error("Logout.Error", "An error occurred during logout")
            );
        }
    }
}
