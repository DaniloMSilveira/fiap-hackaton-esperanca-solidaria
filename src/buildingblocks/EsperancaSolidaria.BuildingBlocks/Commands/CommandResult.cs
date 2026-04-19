

using FluentValidation;
using FluentValidation.Results;

namespace EsperancaSolidaria.BuildingBlocks.Commands;

/// <summary>
/// Resultado padrão para execução de comandos CQRS.
/// </summary>
public class CommandResult
{
    public bool IsValid { get; }
    public IReadOnlyCollection<string> Errors { get; }
    public object? Data { get; }

    private CommandResult(bool isValid, IEnumerable<string> errors, object? data = null)
    {
        IsValid = isValid;
        Errors = errors.ToList();
        Data = data;
    }

    /// <summary>
    /// Cria um resultado de sucesso com dados tipados.
    /// </summary>
    public static CommandResult Success(object? data = null)
        => new CommandResult(true, Array.Empty<string>(), data);

    /// <summary>
    /// Cria um resultado de falha com uma única mensagem.
    /// </summary>
    public static CommandResult Fail(string errorMessage)
        => new CommandResult(false, [errorMessage]);

    /// <summary>
    /// Cria um resultado de falha a partir de um ValidationResult do FluentValidation.
    /// </summary>
    public static CommandResult Fail(ValidationResult validationResult)
        => new CommandResult(false, validationResult.Errors.Select(e => e.ErrorMessage));
}



public class CommandResult<TResponse>
{
    public bool IsValid { get; }
    public IReadOnlyCollection<string> Errors { get; }
    public TResponse? Data { get; }

    private CommandResult(bool isValid, IEnumerable<string> errors, TResponse? data = default)
    {
        IsValid = isValid;
        Errors = errors.ToList();
        Data = data;
    }

    /// <summary>
    /// Cria um resultado de sucesso com dados tipados.
    /// </summary>
    public static CommandResult<TResponse> Success(TResponse data)
        => new CommandResult<TResponse>(true, Array.Empty<string>(), data);

    /// <summary>
    /// Cria um resultado de falha com uma única mensagem.
    /// </summary>
    public static CommandResult<TResponse> Fail(string errorMessage)
        => new CommandResult<TResponse>(false, [errorMessage]);

    /// <summary>
    /// Cria um resultado de falha a partir de um ValidationResult do FluentValidation.
    /// </summary>
    public static CommandResult<TResponse> Fail(ValidationResult validationResult)
        => new CommandResult<TResponse>(false, validationResult.Errors.Select(e => e.ErrorMessage));
}