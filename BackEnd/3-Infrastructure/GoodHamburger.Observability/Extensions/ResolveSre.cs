using System.Diagnostics;
using System.Diagnostics.Metrics;
using GoodHamburger.Observability.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace GoodHamburger.Observability.Extensions;

internal class SreNames(SreConfiguration sreConfiguracao)
{
    internal string ServiceName { get; } = sreConfiguracao.Nome + "_Service";
    internal string ActivitySourceName { get; } = sreConfiguracao.Nome + "_ActivitySource";
    internal string MeterName { get; } = sreConfiguracao.Nome + "_Meter";
}

public static class ResolveSre
{
    private const string Key = "SRE";
    
    public static IServiceCollection AddSre(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
    {
        var sreConfiguration = BuildSreConfiguration(configuration);

        if (sreConfiguration == null || !sreConfiguration.Habilitado) 
            return services;
        
        var randomName = "SRE_" + new Random().Next();
        if (string.IsNullOrEmpty(sreConfiguration.Nome))
            sreConfiguration.Nome = randomName;

        var sreNames = new SreNames(sreConfiguration);
        
        var resource = ResourceBuilder
            .CreateDefault()
            .AddService(
                serviceName: sreNames.ServiceName,
                serviceVersion: typeof(ResolveSre).Assembly.GetName().Version?.ToString() ?? "unknown",
                serviceInstanceId: Environment.MachineName);

        var activitySource = new ActivitySource(sreNames.ActivitySourceName);

        var meter = new Meter(sreNames.MeterName, "1.0.0");

        services.AddOpenTelemetry()
            .ConfigureResource(rb => rb
                .AddService(sreNames.ServiceName, "1.0.0"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(inst =>
                {
                    inst.RecordException = true;
                })
                .AddHttpClientInstrumentation()
                .AddSource(sreNames.ActivitySourceName)
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(sreConfiguration.OtlpEndpointUrl ?? "http://localhost:4317");
                }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter(
                    "Microsoft.AspNetCore.hosting",
                    "Microsoft.AspNetCore.Server.Kestrel",
                    "System.Net.http",
                    "System.Net.NameResolution",
                    meter.Name)
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(sreConfiguration.OtlpEndpointUrl ?? "http://localhost:4317");
                })
                .AddPrometheusExporter());

        builder.Logging.ClearProviders();
        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.IncludeFormattedMessage = true;
            options.SetResourceBuilder(resource);
            
            options.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(sreConfiguration.OtlpEndpointUrl ?? "http://localhost:4317");
            });
            
            if (sreConfiguration.UtilizaConsoleLog)
            {
                options.AddConsoleExporter();
            }
        });
        
        BuildTelemetria(resource, activitySource, meter);
        
        return services;
    }
    
    private static SreConfiguration? BuildSreConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection(Key);
        var sreConfiguration = section.Get<SreConfiguration>();
        return sreConfiguration;
    }
        
    private static void BuildTelemetria(ResourceBuilder resource, ActivitySource activitySource, Meter meter)
    {
        Telemetry.ResourceBuilder = resource;
        Telemetry.ActivitySource = activitySource;
        Telemetry.Meter = meter;
    }

    public static void DefinirUrlPrometheus(this IApplicationBuilder app)
    {
        if (app.ApplicationServices.GetService<MeterProvider>() == null)
            throw new InvalidOperationException("O serviço 'MeterProvider' não foi registrado.");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPrometheusScrapingEndpoint();
        });
    }
}