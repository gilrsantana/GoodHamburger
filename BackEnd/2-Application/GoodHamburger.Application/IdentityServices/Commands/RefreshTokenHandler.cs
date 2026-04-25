using System.IdentityModel.Tokens.Jwt;
using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using GoodHamburger.Application.IdentityServices.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using GoodHamburger.Database.Context;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IRefreshTokenHandler
{
    Task<Result<RefreshTokenResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default);
}

public class RefreshTokenHandler : IRefreshTokenHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IdentityDbContext _dbContext;
    private readonly ILogger<RefreshTokenHandler> _logger;

    public RefreshTokenHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IdentityDbContext dbContext,
        ILogger<RefreshTokenHandler> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<RefreshTokenResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get principal from expired access token
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(command.AccessToken);
            if (principal == null)
            {
                _logger.LogWarning("Invalid access token provided for refresh");
                return Result<RefreshTokenResponse>.Failure(
                    new Error("RefreshToken.InvalidToken", "Invalid access token")
                );
            }

            // Get user ID from principal
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Could not extract user ID from access token");
                return Result<RefreshTokenResponse>.Failure(
                    new Error("RefreshToken.InvalidToken", "Invalid access token")
                );
            }

            // Get JWT token ID from principal
            var jtiClaim = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            // Find the refresh token in database
            var storedRefreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken && rt.UserId == userId, cancellationToken);

            if (storedRefreshToken == null)
            {
                _logger.LogWarning("Refresh token not found for user {UserId}", userId);
                return Result<RefreshTokenResponse>.Failure(
                    new Error("RefreshToken.NotFound", "Refresh token not found")
                );
            }

            // Validate refresh token
            if (storedRefreshToken.IsExpired || storedRefreshToken.IsRevoked)
            {
                _logger.LogWarning("Refresh token is expired or revoked for user {UserId}", userId);
                return Result<RefreshTokenResponse>.Failure(
                    new Error("RefreshToken.Invalid", "Refresh token is expired or revoked")
                );
            }

            // Validate JWT token ID matches
            if (jtiClaim != null && storedRefreshToken.JwtTokenId != jtiClaim)
            {
                _logger.LogWarning("JWT token ID mismatch for user {UserId}", userId);
                return Result<RefreshTokenResponse>.Failure(
                    new Error("RefreshToken.Invalid", "Invalid token combination")
                );
            }

            // Get user
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User not found for refresh token: {UserId}", userId);
                return Result<RefreshTokenResponse>.Failure(
                    new Error("RefreshToken.UserNotFound", "User not found")
                );
            }

            // Get user roles and claims
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            // Generate new tokens
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles, claims);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            var newJwtTokenId = _jwtTokenService.GetJwtTokenId();

            // Revoke old refresh token
            storedRefreshToken.IsRevoked = true;

            // Save new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7), // 7 days
                UserId = user.Id,
                JwtTokenId = newJwtTokenId
            };

            _dbContext.RefreshTokens.Add(newRefreshTokenEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Token refreshed successfully for user {UserId}", userId);

            return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(
                user.Id,
                newAccessToken,
                newRefreshToken,
                DateTime.UtcNow.AddMinutes(15), // Default access token expiration
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during token refresh");
            return Result<RefreshTokenResponse>.Failure(
                new Error("RefreshToken.Error", "An error occurred during token refresh")
            );
        }
    }
}
