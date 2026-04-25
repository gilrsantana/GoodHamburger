using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using GoodHamburger.Application.IdentityServices.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using GoodHamburger.Database.Context;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface ILoginHandler
{
    Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default);
}

public class LoginHandler : ILoginHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IdentityDbContext _dbContext;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IdentityDbContext dbContext,
        ILogger<LoginHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt failed: User with email {Email} not found", command.Email);
                return Result<LoginResponse>.Failure(
                    new Error("Login.Failed", "Invalid email or password")
                );
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: true);
            
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login attempt failed for user {UserId}: {Result}", user.Id, result);
                
                var error = result switch
                {
                    { IsLockedOut: true } => new Error("Login.LockedOut", "Account is locked out"),
                    { IsNotAllowed: true } => new Error("Login.NotAllowed", "Account is not allowed to sign in"),
                    { RequiresTwoFactor: true } => new Error("Login.TwoFactorRequired", "Two-factor authentication is required"),
                    _ => new Error("Login.Failed", "Invalid email or password")
                };
                
                return Result<LoginResponse>.Failure(error);
            }

            // Get user roles and claims
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user, roles, claims);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var jwtTokenId = _jwtTokenService.GetJwtTokenId();

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7), // 7 days
                UserId = user.Id,
                JwtTokenId = jwtTokenId
            };

            _dbContext.RefreshTokens.Add(refreshTokenEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return Result<LoginResponse>.Success(new LoginResponse(
                user.Id,
                accessToken,
                refreshToken,
                DateTime.UtcNow.AddMinutes(15), // Default access token expiration
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for email {Email}", command.Email);
            return Result<LoginResponse>.Failure(
                new Error("Login.Error", "An error occurred during login")
            );
        }
    }
}
