using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.IdentityServices.Commands;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class TokensController : BaseApiController
{
    private readonly IGenerateEmailConfirmationTokenHandler _generateEmailConfirmationTokenHandler;
    private readonly IGeneratePasswordResetTokenHandler _generatePasswordResetTokenHandler;
    private readonly IResetPasswordHandler _resetPasswordHandler;

    public TokensController(
        IGenerateEmailConfirmationTokenHandler generateEmailConfirmationTokenHandler,
        IGeneratePasswordResetTokenHandler generatePasswordResetTokenHandler,
        IResetPasswordHandler resetPasswordHandler)
    {
        _generateEmailConfirmationTokenHandler = generateEmailConfirmationTokenHandler;
        _generatePasswordResetTokenHandler = generatePasswordResetTokenHandler;
        _resetPasswordHandler = resetPasswordHandler;
    }

    [HttpPost("email-confirmation")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenerateEmailConfirmationTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateEmailConfirmationToken([FromBody] GenerateEmailConfirmationTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _generateEmailConfirmationTokenHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpPost("password-reset")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GeneratePasswordResetTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GeneratePasswordResetToken([FromBody] GeneratePasswordResetTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _generatePasswordResetTokenHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _resetPasswordHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }
}
