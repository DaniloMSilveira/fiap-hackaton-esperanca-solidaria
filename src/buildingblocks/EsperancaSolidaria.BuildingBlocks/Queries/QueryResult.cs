using FluentValidation;
using FluentValidation.Results;

namespace EsperancaSolidaria.BuildingBlocks.Queries;

/// <summary>
/// Resultado padrão para execução de queries CQRS.
/// </summary>
public class QueryResult<TResult>
{
    public bool IsValid { get; }
    public TResult? Data { get; }
    public IReadOnlyCollection<string> Errors { get; }

    public QueryResult(bool isValid, IEnumerable<string> errors, TResult? data = default)
    {
        IsValid = isValid;
        Errors = errors.ToList();
        Data = data;
    }

    /// <summary>
    /// Cria um resultado de sucesso com dados tipados.
    /// </summary>
    public static QueryResult<TResult> Success(TResult data)
        => new QueryResult<TResult>(true, Array.Empty<string>(), data);

    /// <summary>
    /// Cria um resultado de falha com uma única mensagem.
    /// </summary>
    public static QueryResult<TResult> Fail(string errorMessage)
        => new QueryResult<TResult>(false, [errorMessage]);

    /// <summary>
    /// Cria um resultado de falha a partir de um ValidationResult do FluentValidation.
    /// </summary>
    public static QueryResult<TResult> Fail(ValidationResult validationResult)
        => new QueryResult<TResult>(false, validationResult.Errors.Select(e => e.ErrorMessage));
}
