namespace EsperancaSolidaria.BuildingBlocks.Queries;

/// <summary>
/// Contrato para manipuladores de consultas.
/// </summary>
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
