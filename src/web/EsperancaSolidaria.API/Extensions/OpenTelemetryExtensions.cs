using System.Diagnostics;

namespace EsperancaSolidaria.API.Extensions;

public static class OpenTelemetryExtensions
{
     public static string ServiceName { get; }
    public static string ServiceVersion { get; }
    public static ActivitySource ActivitySource { get; }

    static OpenTelemetryExtensions()
    {
        ServiceName = "EsperancaSolidaria.Api";
        ServiceVersion = typeof(OpenTelemetryExtensions).Assembly.GetName().Version!.ToString();
        ActivitySource = new ActivitySource(ServiceName, ServiceVersion);
    }
}