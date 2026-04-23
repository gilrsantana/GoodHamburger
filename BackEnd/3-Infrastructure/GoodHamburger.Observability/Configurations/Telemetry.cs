using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Resources;

namespace GoodHamburger.Observability.Configurations;

public static class Telemetry
{
    public static ResourceBuilder? ResourceBuilder { get; internal set; }
    public static ActivitySource? ActivitySource { get; internal set; }
    public static Meter? Meter { get; internal set; }
}