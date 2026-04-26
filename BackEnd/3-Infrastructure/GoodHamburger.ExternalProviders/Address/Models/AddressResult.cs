namespace GoodHamburger.ExternalProviders.Address.Models;

// This class is deprecated - use Result<ViaCepAddressDto> from GoodHamburger.Shared.Handlers instead
[Obsolete("Use Result<ViaCepAddressDto> from GoodHamburger.Shared.Handlers instead")]
public class AddressResult
{
    public bool Success { get; set; }
    public ViaCepAddressDto? Address { get; set; }
    public string? ErrorMessage { get; set; }

    public static AddressResult SuccessResult(ViaCepAddressDto address)
    {
        return new AddressResult
        {
            Success = true,
            Address = address
        };
    }

    public static AddressResult FailureResult(string errorMessage)
    {
        return new AddressResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
