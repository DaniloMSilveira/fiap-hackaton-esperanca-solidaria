using FluentValidation;
using FluentValidation.Results;

namespace EsperancaSolidaria.BuildingBlocks.Queries;

/// <summary>
/// Resultado padrão para execução de queries CQRS.
/// </summary>
public class QueryResult<TResult>
{
    public bool Success { get; }
    public TResult? Data { get; }
    public IReadOnlyCollection<string> Errors { get; }

    /// <summary>
    /// Construtor para resultado bem-sucedido.
    /// </summary>
    public QueryResult(TResult data)
    {
        Success = true;
        Data = data;
        Errors = Array.Empty<string>();
    }

    /// <summary>
    /// Construtor para resultado com falhas de validação.
    /// </summary>
    public QueryResult(ValidationResult validationResult)
    {
        Success = validationResult.IsValid;
        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        Data = default;
    }

    /// <summary>
    /// Método auxiliar para validar parâmetros de uma query.
    /// </summary>
    public static QueryResult<TResult> Validate<TQuery>(TQuery query, IValidator<TQuery> validator)
    {
        var result = validator.Validate(query);
        return new QueryResult<TResult>(result);
    }
}
