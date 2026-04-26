using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.IdentityServices.Commands;
using GoodHamburger.Application.IdentityServices.Queries;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    private readonly ICreateUserHandler _createUserHandler;
    private readonly IUpdateUserHandler _updateUserHandler;
    private readonly IDeleteUserHandler _deleteUserHandler;
    private readonly IGetUserByIdHandler _getUserByIdHandler;
    private readonly IGetAllUsersHandler _getAllUsersHandler;

    public UsersController(
        ICreateUserHandler createUserHandler,
        IUpdateUserHandler updateUserHandler,
        IDeleteUserHandler deleteUserHandler,
        IGetUserByIdHandler getUserByIdHandler,
        IGetAllUsersHandler getAllUsersHandler)
    {
        _createUserHandler = createUserHandler;
        _updateUserHandler = updateUserHandler;
        _deleteUserHandler = deleteUserHandler;
        _getUserByIdHandler = getUserByIdHandler;
        _getAllUsersHandler = getAllUsersHandler;
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _createUserHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(string id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _getUserByIdHandler.HandleAsync(query, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpGet]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetAllUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery(page, pageSize, search);
        var result = await _getAllUsersHandler.HandleAsync(query, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { UserId = id };
        var result = await _updateUserHandler.HandleAsync(updateCommand, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(DeleteUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await _deleteUserHandler.HandleAsync(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }
}
