using GoodHamburger.ExternalProviders.Address.Models;
using GoodHamburger.Shared.Handlers;

namespace GoodHamburger.ExternalProviders.Address;

public interface IViaCepService
{
    Task<Result<ViaCepAddressDto>> GetAddressByZipCodeAsync(string zipCode, CancellationToken cancellationToken = default);
}
