using System.ComponentModel;
using System.Reflection;

namespace EsperancaSolidaria.BuildingBlocks.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Obtém a descrição de um valor de Enum usando o atributo [Description].
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? value.ToString();
    }
}