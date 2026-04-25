using EsperancaSolidaria.Application.Queries.Campanhas.Results;
using EsperancaSolidaria.BuildingBlocks.Queries;

namespace EsperancaSolidaria.Application.Queries.Campanhas.Handlers;

public interface ICampanhaQueryHandler
{
    Task<PaginatedResult<CampanhaListaResult>> HandleAsync(ConsultarCampanhasQuery query, CancellationToken cancellationToken = default);
    Task<CampanhaDetalhesResult?> HandleAsync(ObterCampanhaPorIdQuery query, CancellationToken cancellationToken = default);
    Task<PaginatedResult<CampanhaPublicaResult>> HandleAsync(ConsultarCampanhasAtivasQuery query, CancellationToken cancellationToken = default);
}
