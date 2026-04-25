

using FluentValidation;
using FluentValidation.Results;

namespace EsperancaSolidaria.BuildingBlocks.Commands;

/// <summary>
/// Resultado padrão para execução de comandos CQRS.
/// </summary>
public class CommandResult<T>
{
    public bool IsValid { get; }
    public IReadOnlyCollection<string> Errors { get; }
    public T? Data { get; }

    protected CommandResult(bool isValid, IEnumerable<string> errors, T? data = default)
    {
        IsValid = isValid;
        Errors = errors.ToList();
        Data = data;
    }

    public static CommandResult<T> Success(T? data = default)
        => new CommandResult<T>(true, Array.Empty<string>(), data);

    public static CommandResult<T> Fail(string errorMessage)
        => new CommandResult<T>(false, new[] { errorMessage });

    public static CommandResult<T> Fail(ValidationResult validationResult)
        => new CommandResult<T>(false, validationResult.Errors.Select(e => e.ErrorMessage));
}

/// <summary>
/// Alias para CommandResult sem dados, quando o resultado é apenas sucesso ou falha
/// </summary>
public class CommandResult : CommandResult<object>
{
    protected CommandResult(bool isValid, IEnumerable<string> errors, object? data = null)
        : base(isValid, errors, data) { }

    public static new CommandResult Success()
        => new CommandResult(true, Array.Empty<string>(), null);

    public static new CommandResult Fail(string errorMessage)
        => new CommandResult(false, new[] { errorMessage });

    public static new CommandResult Fail(ValidationResult validationResult)
        => new CommandResult(false, validationResult.Errors.Select(e => e.ErrorMessage));
}
