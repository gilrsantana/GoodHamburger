using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.IdentityServices.Commands;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class ClaimsController : BaseApiController
{
    private readonly IAddClaimToUserHandler _addClaimToUserHandler;
    private readonly IRemoveClaimFromUserHandler _removeClaimFromUserHandler;

    public ClaimsController(
        IAddClaimToUserHandler addClaimToUserHandler,
        IRemoveClaimFromUserHandler removeClaimFromUserHandler)
    {
        _addClaimToUserHandler = addClaimToUserHandler;
        _removeClaimFromUserHandler = removeClaimFromUserHandler;
    }

    [HttpPost("add")]
    [ProducesResponseType(typeof(AddClaimToUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddClaimToUser([FromBody] AddClaimToUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _addClaimToUserHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpPost("remove")]
    [ProducesResponseType(typeof(RemoveClaimFromUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveClaimFromUser([FromBody] RemoveClaimFromUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _removeClaimFromUserHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }
}
