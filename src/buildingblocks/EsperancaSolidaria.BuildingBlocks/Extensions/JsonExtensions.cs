using System.Globalization;
using System.Text.Json;

namespace EsperancaSolidaria.BuildingBlocks.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    static JsonExtensions()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");
    }

    /// <summary>
    /// Serializa um objeto para JSON em camelCase.
    /// </summary>
    public static string Serialize(this object obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    /// <summary>
    /// Desserializa JSON para objeto, aceitando camelCase ou PascalCase.
    /// </summary>
    public static T? Deserialize<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
