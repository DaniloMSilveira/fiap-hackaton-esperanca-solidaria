using System.Text;

namespace EsperancaSolidaria.BuildingBlocks.Extensions;

/// <summary>
/// Obtém a stack trace completa de uma exceção, incluindo inner exceptions.
/// </summary>
public static class ExceptionExtensions
{
    public static string GetFullStackTrace(this Exception ex)
    {
        var sb = new StringBuilder();
        sb.AppendLine(ex.ToString());

        var inner = ex.InnerException;
        while (inner != null)
        {
            sb.AppendLine("Inner Exception:");
            sb.AppendLine(inner.ToString());
            inner = inner.InnerException;
        }

        return sb.ToString();
    }
}

