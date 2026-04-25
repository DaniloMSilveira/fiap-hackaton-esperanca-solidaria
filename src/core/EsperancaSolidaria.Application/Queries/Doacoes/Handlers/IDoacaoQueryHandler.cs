using EsperancaSolidaria.Application.Queries.Doacoes.Results;
using EsperancaSolidaria.BuildingBlocks.Queries;

namespace EsperancaSolidaria.Application.Queries.Doacoes.Handlers;

public interface IDoacaoQueryHandler
{
    Task<PaginatedResult<DoacaoResult>> HandleAsync(ConsultarDoacoesQuery query, CancellationToken cancellationToken = default);
}