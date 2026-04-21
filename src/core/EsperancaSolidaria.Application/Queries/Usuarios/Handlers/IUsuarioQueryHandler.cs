using EsperancaSolidaria.Application.Queries.Usuarios.Inputs;
using EsperancaSolidaria.BuildingBlocks.Queries;

namespace EsperancaSolidaria.Application.Queries.Usuarios.Handlers;

public interface IUsuarioQueryHandler
{
    Task<QueryResult<PaginatedResult<UsuarioListaResult>>> HandleAsync(ConsultarUsuariosQuery query, CancellationToken cancellationToken = default);
    Task<QueryResult<UsuarioResult>> HandleAsync(ObterUsuarioPorIdQuery query, CancellationToken cancellationToken = default);
}
