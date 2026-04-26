using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.AccountServices.Commands;
using GoodHamburger.Application.AccountServices.Queries;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Accounts;

[ApiController]
[Route("api/[controller]")]
public class EmployeeProfilesController : BaseApiController
{
    private readonly ICreateEmployeeProfileHandler _createEmployeeProfileHandler;
    private readonly IUpdateEmployeeProfileHandler _updateEmployeeProfileHandler;
    private readonly IUpdateEmployeeCodeHandler _updateEmployeeCodeHandler;
    private readonly IUpdateEmployeeRoleTitleHandler _updateEmployeeRoleTitleHandler;
    private readonly IDeleteEmployeeProfileHandler _deleteEmployeeProfileHandler;
    private readonly IGetEmployeeProfileByIdHandler _getEmployeeProfileByIdHandler;
    private readonly IGetEmployeeProfileByIdentityIdHandler _getEmployeeProfileByIdentityIdHandler;
    private readonly IGetAllEmployeeProfilesHandler _getAllEmployeeProfilesHandler;
    private readonly IGetActiveEmployeeProfilesHandler _getActiveEmployeeProfilesHandler;

    public EmployeeProfilesController(
        ICreateEmployeeProfileHandler createEmployeeProfileHandler,
        IUpdateEmployeeProfileHandler updateEmployeeProfileHandler,
        IUpdateEmployeeCodeHandler updateEmployeeCodeHandler,
        IUpdateEmployeeRoleTitleHandler updateEmployeeRoleTitleHandler,
        IDeleteEmployeeProfileHandler deleteEmployeeProfileHandler,
        IGetEmployeeProfileByIdHandler getEmployeeProfileByIdHandler,
        IGetEmployeeProfileByIdentityIdHandler getEmployeeProfileByIdentityIdHandler,
        IGetAllEmployeeProfilesHandler getAllEmployeeProfilesHandler,
        IGetActiveEmployeeProfilesHandler getActiveEmployeeProfilesHandler)
    {
        _createEmployeeProfileHandler = createEmployeeProfileHandler;
        _updateEmployeeProfileHandler = updateEmployeeProfileHandler;
        _updateEmployeeCodeHandler = updateEmployeeCodeHandler;
        _updateEmployeeRoleTitleHandler = updateEmployeeRoleTitleHandler;
        _deleteEmployeeProfileHandler = deleteEmployeeProfileHandler;
        _getEmployeeProfileByIdHandler = getEmployeeProfileByIdHandler;
        _getEmployeeProfileByIdentityIdHandler = getEmployeeProfileByIdentityIdHandler;
        _getAllEmployeeProfilesHandler = getAllEmployeeProfilesHandler;
        _getActiveEmployeeProfilesHandler = getActiveEmployeeProfilesHandler;
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(CreateEmployeeProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEmployeeProfile([FromBody] CreateEmployeeProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await _createEmployeeProfileHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateEmployeeProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEmployeeProfile(Guid id, [FromBody] UpdateEmployeeProfileCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { EmployeeProfileId = id };
        var result = await _updateEmployeeProfileHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{id}/code")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateEmployeeCodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateEmployeeCode(Guid id, [FromBody] UpdateEmployeeCodeCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { EmployeeProfileId = id };
        var result = await _updateEmployeeCodeHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{id}/role-title")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(UpdateEmployeeRoleTitleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEmployeeRoleTitle(Guid id, [FromBody] UpdateEmployeeRoleTitleCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { EmployeeProfileId = id };
        var result = await _updateEmployeeRoleTitleHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(DeleteEmployeeProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEmployeeProfile(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteEmployeeProfileCommand(id);
        var result = await _deleteEmployeeProfileHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetEmployeeProfileByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeProfileById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeProfileByIdQuery(id);
        var result = await _getEmployeeProfileByIdHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("by-identity/{identityId}")]
    [Authorize]
    [ProducesResponseType(typeof(GetEmployeeProfileByIdentityIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeProfileByIdentityId(Guid identityId, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeProfileByIdentityIdQuery(identityId);
        var result = await _getEmployeeProfileByIdentityIdHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetAllEmployeeProfilesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllEmployeeProfiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllEmployeeProfilesQuery(page, pageSize, search);
        var result = await _getAllEmployeeProfilesHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("active")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetActiveEmployeeProfilesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetActiveEmployeeProfiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetActiveEmployeeProfilesQuery(page, pageSize);
        var result = await _getActiveEmployeeProfilesHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
}
