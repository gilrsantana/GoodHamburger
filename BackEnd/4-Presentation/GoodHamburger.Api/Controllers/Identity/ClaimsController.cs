using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.IdentityServices.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ClaimsController : ApiController
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
    public async Task<IActionResult> AddClaimToUser([FromBody] AddClaimToUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _addClaimToUserHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveClaimFromUser([FromBody] RemoveClaimFromUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _removeClaimFromUserHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }
}
