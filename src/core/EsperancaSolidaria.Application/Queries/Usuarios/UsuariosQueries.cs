using EsperancaSolidaria.BuildingBlocks.Queries;

namespace EsperancaSolidaria.Application.Queries.Usuarios;

public record ObterUsuarioPorIdQuery(Guid Id) : IQuery;

public record ConsultarUsuariosQuery(
    int Pagina,
    int TamanhoPagina,
    string? Nome = null,
    string? Email = null
) : IQuery;
