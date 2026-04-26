using GoodHamburger.Api.Controllers.Base;
using GoodHamburger.ExternalProviders.Address;
using GoodHamburger.ExternalProviders.Address.Models;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Address;

[ApiController]
[Route("api/[controller]")]
public class AddressController : BaseApiController
{
    private readonly IViaCepService _viaCepService;

    public AddressController(IViaCepService viaCepService)
    {
        _viaCepService = viaCepService;
    }

    [HttpGet("via-cep/{zipCode}")]
    [ProducesResponseType(typeof(ViaCepAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAddressByZipCode(string zipCode, CancellationToken cancellationToken)
    {
        var result = await _viaCepService.GetAddressByZipCodeAsync(zipCode, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
}
