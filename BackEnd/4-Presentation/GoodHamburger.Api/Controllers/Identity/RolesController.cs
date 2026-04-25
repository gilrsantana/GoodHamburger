using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.IdentityServices.Commands;
using GoodHamburger.Application.IdentityServices.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ApiController
{
    private readonly ICreateRoleHandler _createRoleHandler;
    private readonly IAssignRoleToUserHandler _assignRoleToUserHandler;
    private readonly IRemoveRoleFromUserHandler _removeRoleFromUserHandler;
    private readonly IGetAllRolesHandler _getAllRolesHandler;

    public RolesController(
        ICreateRoleHandler createRoleHandler,
        IAssignRoleToUserHandler assignRoleToUserHandler,
        IRemoveRoleFromUserHandler removeRoleFromUserHandler,
        IGetAllRolesHandler getAllRolesHandler)
    {
        _createRoleHandler = createRoleHandler;
        _assignRoleToUserHandler = assignRoleToUserHandler;
        _removeRoleFromUserHandler = removeRoleFromUserHandler;
        _getAllRolesHandler = getAllRolesHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var result = await _createRoleHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var query = new GetAllRolesQuery();
        var result = await _getAllRolesHandler.HandleAsync(query, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _assignRoleToUserHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleFromUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _removeRoleFromUserHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }
}
