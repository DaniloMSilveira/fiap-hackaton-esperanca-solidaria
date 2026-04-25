using EsperancaSolidaria.BuildingBlocks.Queries;

namespace EsperancaSolidaria.Application.Queries.Doacoes;

public record ConsultarDoacoesQuery(
    int Pagina,
    int TamanhoPagina,
    Guid CampanhaId
) : IQuery;