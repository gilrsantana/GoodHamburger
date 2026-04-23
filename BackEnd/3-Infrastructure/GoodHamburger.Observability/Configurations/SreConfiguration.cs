
namespace GoodHamburger.Observability.Configurations;

public class SreConfiguration
{
    public bool Habilitado { get; init; }
    public string? OtlpEndpointUrl { get; init; }
    public bool UtilizaConsoleLog { get; init; }
    public string? Nome { get; set; }
}