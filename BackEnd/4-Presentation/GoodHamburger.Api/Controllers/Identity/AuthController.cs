using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.IdentityServices.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GoodHamburger.Shared.Handlers;

namespace GoodHamburger.Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly ILoginHandler _loginHandler;
    private readonly IRefreshTokenHandler _refreshTokenHandler;
    private readonly ILogoutHandler _logoutHandler;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ILoginHandler loginHandler,
        IRefreshTokenHandler refreshTokenHandler,
        ILogoutHandler logoutHandler, ILogger<AuthController> logger)
    {
        _loginHandler = loginHandler;
        _refreshTokenHandler = refreshTokenHandler;
        _logoutHandler = logoutHandler;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _loginHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _refreshTokenHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? 
                          User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null)
        {
            _logger.LogWarning("User ID not found in token");
            var resultError = Result<LogoutResponse>.Failure(
                new Error("Logout.UserNotFound", "User ID not found in token")
            );
            return HandleFailure(resultError);
        }

        var command = new LogoutCommand(userIdClaim.Value);
        var result = await _logoutHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }
}
