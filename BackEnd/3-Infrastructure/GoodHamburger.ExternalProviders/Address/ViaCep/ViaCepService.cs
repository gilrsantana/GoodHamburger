using System.Text.Json;
using GoodHamburger.ExternalProviders.Address.Models;
using GoodHamburger.ExternalProviders.Address.ViaCep.Models;
using GoodHamburger.Shared.Handlers;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.ExternalProviders.Address.ViaCep;

public class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ViaCepService> _logger;
    private const string BaseUrl = "https://viacep.com.br";

    public ViaCepService(HttpClient httpClient, ILogger<ViaCepService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<ViaCepAddressDto>> GetAddressByZipCodeAsync(string zipCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var cleanedZipCode = new string(zipCode.Where(char.IsDigit).ToArray());

            if (cleanedZipCode.Length != 8)
            {
                _logger.LogWarning("Invalid zip code format: {ZipCode}", zipCode);
                return Result<ViaCepAddressDto>.Failure(
                    new Error("ViaCep.ZipCode.Validation", "Invalid zip code format. Must contain 8 digits."));
            }

            var url = $"{BaseUrl}/ws/{cleanedZipCode}/json";

            _logger.LogInformation("Fetching address from ViaCEP for zip code: {ZipCode}", cleanedZipCode);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ViaCEP returned status code: {StatusCode} for zip code: {ZipCode}", 
                    response.StatusCode, cleanedZipCode);
                return Result<ViaCepAddressDto>.Failure(
                    new Error("ViaCep.ServiceError", $"ViaCEP returned status code: {(int)response.StatusCode}"));
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (content.Contains("\"erro\":true"))
            {
                _logger.LogWarning("ViaCEP returned error for zip code: {ZipCode}", cleanedZipCode);
                return Result<ViaCepAddressDto>.Failure(
                    new Error("ViaCep.NotFound", "Zip code not found."));
            }

            var viaCepResponse = JsonSerializer.Deserialize<ViaCepAddressResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (viaCepResponse == null)
            {
                _logger.LogError("Failed to deserialize ViaCEP response for zip code: {ZipCode}", cleanedZipCode);
                return Result<ViaCepAddressDto>.Failure(
                    new Error("ViaCep.DeserializationError", "Failed to deserialize ViaCEP response."));
            }

            var address = new ViaCepAddressDto
            {
                ZipCode = viaCepResponse.Cep,
                Street = viaCepResponse.Logradouro,
                Complement = viaCepResponse.Complemento,
                District = viaCepResponse.Bairro,
                City = viaCepResponse.Localidade,
                State = viaCepResponse.Uf
            };

            _logger.LogInformation("Successfully fetched address for zip code: {ZipCode}", cleanedZipCode);

            return Result<ViaCepAddressDto>.Success(address);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching address from ViaCEP for zip code: {ZipCode}", zipCode);
            return Result<ViaCepAddressDto>.Failure(
                new Error("ViaCep.HttpError", "HTTP error occurred while fetching address."));
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error occurred while parsing ViaCEP response for zip code: {ZipCode}", zipCode);
            return Result<ViaCepAddressDto>.Failure(
                new Error("ViaCep.JsonError", "Error parsing ViaCEP response."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching address from ViaCEP for zip code: {ZipCode}", zipCode);
            return Result<ViaCepAddressDto>.Failure(
                new Error("ViaCep.UnexpectedError", "An unexpected error occurred."));
        }
    }
}
