using EsperancaSolidaria.Application.Queries.Usuarios.Inputs;
using EsperancaSolidaria.BuildingBlocks.Extensions;
using EsperancaSolidaria.BuildingBlocks.Queries;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using FluentValidation;

namespace EsperancaSolidaria.Application.Queries.Usuarios.Handlers;

public class UsuarioQueryHandler : IUsuarioQueryHandler
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioQueryHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<QueryResult<PaginatedResult<UsuarioListaResult>>> HandleAsync(ConsultarUsuariosQuery query, CancellationToken cancellationToken = default)
    {
        var queryValidation = query.Validate();
        if (!queryValidation.IsValid)
            return QueryResult<PaginatedResult<UsuarioListaResult>>.Fail(queryValidation);

        var usuarios = await _usuarioRepository.ConsultarUsuariosAsync(query.Nome, query.Email, cancellationToken);

        var usuariosPaginado = usuarios
            .Skip((query.Pagina - 1) * query.TamanhoPagina)
            .Take(query.TamanhoPagina)
            .Select(u => new UsuarioListaResult
            {
                Id = u.Id,
                NomeCompleto = u.NomeCompleto,
                Email = u.Email.Value,
                DataCriacao = u.DataCriacao,
                Ativo = u.Ativo
            })
            .ToList();

        var resultadoPaginado = new PaginatedResult<UsuarioListaResult>(
            page: query.Pagina,
            pageSize: query.TamanhoPagina,
            totalItems: usuarios.Count(),
            items: usuariosPaginado
        );

        return QueryResult<PaginatedResult<UsuarioListaResult>>.Success(resultadoPaginado);
    }

    public async Task<QueryResult<UsuarioResult>> HandleAsync(ObterUsuarioPorIdQuery query, CancellationToken cancellationToken = default)
    {
        var queryValidation = query.Validate();
        if (!queryValidation.IsValid)
            return QueryResult<UsuarioResult>.Fail(queryValidation);

        var usuario = await _usuarioRepository.ObterPorIdAsync(query.Id, cancellationToken);
        if (usuario == null)
            return QueryResult<UsuarioResult>.Fail("Usuário não encontrado.");

        var usuarioResult = new UsuarioResult
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

        return QueryResult<UsuarioResult>.Success(usuarioResult);
    }
}
