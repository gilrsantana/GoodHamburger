using System.Security.Claims;
using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.Application.AccountServices.Commands;
using GoodHamburger.Application.AccountServices.Queries;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Accounts;

[ApiController]
[Route("api/[controller]")]
public class CustomerProfilesController : BaseApiController
{
    private readonly ICreateCustomerProfileHandler _createCustomerProfileHandler;
    private readonly IUpdateCustomerProfileHandler _updateCustomerProfileHandler;
    private readonly IUpdateCustomerDocumentHandler _updateCustomerDocumentHandler;
    private readonly IUpdateCustomerAddressHandler _updateCustomerAddressHandler;
    private readonly IUpdateCustomerBirthDateHandler _updateCustomerBirthDateHandler;
    private readonly IDeleteCustomerProfileHandler _deleteCustomerProfileHandler;
    private readonly IGetCustomerProfileByIdHandler _getCustomerProfileByIdHandler;
    private readonly IGetCustomerProfileByIdentityIdHandler _getCustomerProfileByIdentityIdHandler;
    private readonly IGetCustomerProfileByDocumentHandler _getCustomerProfileByDocumentHandler;
    private readonly IGetAllCustomerProfilesHandler _getAllCustomerProfilesHandler;
    private readonly IGetActiveCustomerProfilesHandler _getActiveCustomerProfilesHandler;

    public CustomerProfilesController(
        ICreateCustomerProfileHandler createCustomerProfileHandler,
        IUpdateCustomerProfileHandler updateCustomerProfileHandler,
        IUpdateCustomerDocumentHandler updateCustomerDocumentHandler,
        IUpdateCustomerAddressHandler updateCustomerAddressHandler,
        IUpdateCustomerBirthDateHandler updateCustomerBirthDateHandler,
        IDeleteCustomerProfileHandler deleteCustomerProfileHandler,
        IGetCustomerProfileByIdHandler getCustomerProfileByIdHandler,
        IGetCustomerProfileByIdentityIdHandler getCustomerProfileByIdentityIdHandler,
        IGetCustomerProfileByDocumentHandler getCustomerProfileByDocumentHandler,
        IGetAllCustomerProfilesHandler getAllCustomerProfilesHandler,
        IGetActiveCustomerProfilesHandler getActiveCustomerProfilesHandler)
    {
        _createCustomerProfileHandler = createCustomerProfileHandler;
        _updateCustomerProfileHandler = updateCustomerProfileHandler;
        _updateCustomerDocumentHandler = updateCustomerDocumentHandler;
        _updateCustomerAddressHandler = updateCustomerAddressHandler;
        _updateCustomerBirthDateHandler = updateCustomerBirthDateHandler;
        _deleteCustomerProfileHandler = deleteCustomerProfileHandler;
        _getCustomerProfileByIdHandler = getCustomerProfileByIdHandler;
        _getCustomerProfileByIdentityIdHandler = getCustomerProfileByIdentityIdHandler;
        _getCustomerProfileByDocumentHandler = getCustomerProfileByDocumentHandler;
        _getAllCustomerProfilesHandler = getAllCustomerProfilesHandler;
        _getActiveCustomerProfilesHandler = getActiveCustomerProfilesHandler;
    }

    [HttpPost]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(CreateCustomerProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCustomerProfile([FromBody] CreateCustomerProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await _createCustomerProfileHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateCustomerProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCustomerProfile(Guid id, [FromBody] UpdateCustomerProfileCommand command, CancellationToken cancellationToken)
    {
        if (UserInValidToPerformAction(id))
        {
            return HandleFailure(Result<UpdateCustomerProfileResponse>.Failure(
                new Error("Customer.Unauthorized", "You are not authorized to update this customer profile.")));
        }
        var updateCommand = command with { IdentityId = id };
        var result = await _updateCustomerProfileHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    

    [HttpPut("{id}/document")]
    [ProducesResponseType(typeof(UpdateCustomerDocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCustomerDocument(Guid id, [FromBody] UpdateCustomerDocumentCommand command, CancellationToken cancellationToken)
    {
        if (UserInValidToPerformAction(id))
        {
            return HandleFailure(Result<UpdateCustomerProfileResponse>.Failure(
                new Error("Customer.Unauthorized", "You are not authorized to update this customer profile.")));
        }
        var updateCommand = command with { IdentityId = id };
        var result = await _updateCustomerDocumentHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{id}/address")]
    [ProducesResponseType(typeof(UpdateCustomerAddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCustomerAddress(Guid id, [FromBody] UpdateCustomerAddressCommand command, CancellationToken cancellationToken)
    {
        if (UserInValidToPerformAction(id))
        {
            return HandleFailure(Result<UpdateCustomerProfileResponse>.Failure(
                new Error("Customer.Unauthorized", "You are not authorized to update this customer profile.")));
        }
        var updateCommand = command with { IdentityId = id };
        var result = await _updateCustomerAddressHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpPut("{id}/birth-date")]
    [ProducesResponseType(typeof(UpdateCustomerBirthDateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCustomerBirthDate(Guid id, [FromBody] UpdateCustomerBirthDateCommand command, CancellationToken cancellationToken)
    {
        if (UserInValidToPerformAction(id))
        {
            return HandleFailure(Result<UpdateCustomerProfileResponse>.Failure(
                new Error("Customer.Unauthorized", "You are not authorized to update this customer profile.")));
        }
        var updateCommand = command with { IdentityId = id };
        var result = await _updateCustomerBirthDateHandler.HandleAsync(updateCommand, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(DeleteCustomerProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomerProfile(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCustomerProfileCommand(id);
        var result = await _deleteCustomerProfileHandler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(GetCustomerProfileByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerProfileById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCustomerProfileByIdQuery(id);
        var result = await _getCustomerProfileByIdHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("by-identity/{identityId}")]
    [Authorize]
    [ProducesResponseType(typeof(GetCustomerProfileByIdentityIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerProfileByIdentityId(Guid identityId, CancellationToken cancellationToken)
    {
        var query = new GetCustomerProfileByIdentityIdQuery(identityId);
        var result = await _getCustomerProfileByIdentityIdHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("by-document/{documentNumber}")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetCustomerProfileByDocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerProfileByDocument(string documentNumber, CancellationToken cancellationToken)
    {
        var query = new GetCustomerProfileByDocumentQuery(documentNumber);
        var result = await _getCustomerProfileByDocumentHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetAllCustomerProfilesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllCustomerProfiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCustomerProfilesQuery(page, pageSize, search);
        var result = await _getAllCustomerProfilesHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("active")]
    [Authorize(Policy = "Management")]
    [ProducesResponseType(typeof(GetActiveCustomerProfilesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetActiveCustomerProfiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetActiveCustomerProfilesQuery(page, pageSize);
        var result = await _getActiveCustomerProfilesHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
}
