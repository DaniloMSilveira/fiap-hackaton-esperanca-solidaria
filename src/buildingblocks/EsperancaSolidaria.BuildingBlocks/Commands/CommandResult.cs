

using FluentValidation;
using FluentValidation.Results;

namespace EsperancaSolidaria.BuildingBlocks.Commands;

/// <summary>
/// Resultado padrão para execução de comandos CQRS.
/// </summary>
public class CommandResult
{
    public bool Success { get; }
    public IReadOnlyCollection<string> Errors { get; }
    public object? Data { get; }

    /// <summary>
    /// Construtor para resultado bem-sucedido.
    /// </summary>
    public CommandResult(object? data = null)
    {
        Success = true;
        Errors = Array.Empty<string>();
        Data = data;
    }

    /// <summary>
    /// Construtor para resultado com falhas de validação.
    /// </summary>
    public CommandResult(ValidationResult validationResult)
    {
        Success = validationResult.IsValid;
        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        Data = null;
    }

    /// <summary>
    /// Método auxiliar para validar parâmetros de um comando.
    /// </summary>
    public static CommandResult Validate<TCommand>(TCommand command, IValidator<TCommand> validator)
    {
        var result = validator.Validate(command);
        return new CommandResult(result);
    }
}

