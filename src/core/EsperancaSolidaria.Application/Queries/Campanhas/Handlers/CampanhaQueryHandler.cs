using EsperancaSolidaria.Application.Queries.Campanhas.Results;
using EsperancaSolidaria.BuildingBlocks.Extensions;
using EsperancaSolidaria.BuildingBlocks.Queries;
using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.Interfaces.Repositories;

namespace EsperancaSolidaria.Application.Queries.Campanhas.Handlers;

public class CampanhaQueryHandler : ICampanhaQueryHandler
{
    private readonly ICampanhaRepository _campanhaRepository;

    public CampanhaQueryHandler(ICampanhaRepository campanhaRepository)
    {
        _campanhaRepository = campanhaRepository;
    }

    public async Task<PaginatedResult<CampanhaListaResult>> HandleAsync(ConsultarCampanhasQuery query, CancellationToken cancellationToken = default)
    {
        var campanhas = await _campanhaRepository.ObterTodosAsync();

        var resultado = campanhas
            .Skip((query.Pagina - 1) * query.TamanhoPagina)
            .Take(query.TamanhoPagina)
            .Select(c => new CampanhaListaResult(
                c.Id,
                c.Titulo,
                c.DataInicio,
                c.DataFim,
                c.MetaFinanceira,
                c.ValorArrecadado,
                c.Status,
                c.Status.GetDescription(),
                c.DataCriacao
            )).ToList();

        return new PaginatedResult<CampanhaListaResult>(query.Pagina, query.TamanhoPagina, campanhas.Count(), resultado);
    }

    public async Task<CampanhaDetalhesResult?> HandleAsync(ObterCampanhaPorIdQuery query, CancellationToken cancellationToken)
    {
        var campanha = await _campanhaRepository.ObterPorIdAsync(query.Id);
        if (campanha is null)
            return null;

        return new CampanhaDetalhesResult(
            campanha.Id,
            campanha.Titulo,
            campanha.Descricao,
            campanha.DataInicio,
            campanha.DataFim,
            campanha.MetaFinanceira,
            campanha.ValorArrecadado,
            campanha.Status,
            campanha.Status.GetDescription(),
            campanha.DataCriacao,
            campanha.UsuarioCriacao
        );
    }

    public async Task<PaginatedResult<CampanhaPublicaResult>> HandleAsync(ConsultarCampanhasAtivasQuery query, CancellationToken cancellationToken = default)
    {
        var campanhas = await _campanhaRepository.ObterPorStatusAsync(EStatusCampanha.Ativa);

        var resultado = campanhas
            .Skip((query.Pagina - 1) * query.TamanhoPagina)
            .Take(query.TamanhoPagina)
            .Select(c => new CampanhaPublicaResult(
                c.Id,
                c.Titulo,
                c.Descricao,
                c.MetaFinanceira,
                c.ValorArrecadado,
                c.DataInicio,
                c.DataFim
            )).ToList();

        return new PaginatedResult<CampanhaPublicaResult>(query.Pagina, query.TamanhoPagina, campanhas.Count(), resultado);
    }
}
