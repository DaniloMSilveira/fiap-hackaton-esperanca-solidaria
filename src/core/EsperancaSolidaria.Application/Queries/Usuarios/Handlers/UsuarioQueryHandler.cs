using EsperancaSolidaria.Application.Queries.Usuarios.Results;
using EsperancaSolidaria.BuildingBlocks.Extensions;
using EsperancaSolidaria.BuildingBlocks.Queries;
using EsperancaSolidaria.Domain.Interfaces.Repositories;

namespace EsperancaSolidaria.Application.Queries.Usuarios.Handlers;

public class UsuarioQueryHandler : IUsuarioQueryHandler
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioQueryHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<PaginatedResult<UsuarioListaQueryResult>> HandleAsync(ConsultarUsuariosQuery query, CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarioRepository.ConsultarUsuariosAsync(query.Nome, query.Email, cancellationToken);

        var usuariosPaginado = usuarios
            .Skip((query.Pagina - 1) * query.TamanhoPagina)
            .Take(query.TamanhoPagina)
            .Select(u => new UsuarioListaQueryResult
            {
                Id = u.Id,
                NomeCompleto = u.NomeCompleto,
                Email = u.Email.Value,
                DataCriacao = u.DataCriacao,
                Ativo = u.Ativo
            })
            .ToList();

        var resultadoPaginado = new PaginatedResult<UsuarioListaQueryResult>(
            page: query.Pagina,
            pageSize: query.TamanhoPagina,
            totalItems: usuarios.Count(),
            items: usuariosPaginado
        );

        return resultadoPaginado;
    }

    public async Task<UsuarioQueryResult?> HandleAsync(ObterUsuarioPorIdQuery query, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(query.Id, cancellationToken);
        if (usuario == null)
            return null;

        var resultado = new UsuarioQueryResult
        {
            Id = usuario.Id,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email.Value,
            Cpf = usuario.Cpf.Value,
            PerfilAcessoId = usuario.PerfilAcesso,
            PerfilAcessoDescricao = usuario.PerfilAcesso.GetDescription(),
            DataCriacao = usuario.DataCriacao,
            Ativo = usuario.Ativo
        };

        return resultado;
    }
}
