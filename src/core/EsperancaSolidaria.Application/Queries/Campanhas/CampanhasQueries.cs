using EsperancaSolidaria.BuildingBlocks.Queries;

namespace EsperancaSolidaria.Application.Queries.Campanhas;

public record ConsultarCampanhasQuery(
    int Pagina,
    int TamanhoPagina,
    string? Titulo
) : IQuery;

public record ObterCampanhaPorIdQuery(Guid Id) : IQuery;

public record ConsultarCampanhasAtivasQuery(
    int Pagina,
    int TamanhoPagina
) : IQuery;
