using EsperancaSolidaria.Application.Queries.Usuarios.Results;
using EsperancaSolidaria.BuildingBlocks.Queries;

namespace EsperancaSolidaria.Application.Queries.Usuarios.Handlers;

public interface IUsuarioQueryHandler
{
    Task<PaginatedResult<UsuarioListaQueryResult>> HandleAsync(ConsultarUsuariosQuery query, CancellationToken cancellationToken = default);
    Task<UsuarioQueryResult?> HandleAsync(ObterUsuarioPorIdQuery query, CancellationToken cancellationToken = default);
}
