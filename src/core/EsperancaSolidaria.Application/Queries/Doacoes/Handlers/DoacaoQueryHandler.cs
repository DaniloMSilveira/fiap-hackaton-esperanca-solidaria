using EsperancaSolidaria.Application.Queries.Doacoes.Results;
using EsperancaSolidaria.BuildingBlocks.Queries;
using EsperancaSolidaria.Domain.Interfaces.Repositories;

namespace EsperancaSolidaria.Application.Queries.Doacoes.Handlers;

public class DoacaoQueryHandler : IDoacaoQueryHandler
{
    private readonly IDoacaoRepository _doacaoRepository;

    public DoacaoQueryHandler(IDoacaoRepository doacaoRepository)
    {
        _doacaoRepository = doacaoRepository;
    }

    public async Task<PaginatedResult<DoacaoResult>> HandleAsync(ConsultarDoacoesQuery query, CancellationToken cancellationToken = default)
    {
        var doacoes = await _doacaoRepository.ObterDoacoesAsync(query.CampanhaId);

        var resultado = doacoes
            .Skip((query.Pagina - 1) * query.TamanhoPagina)
            .Take(query.TamanhoPagina)
            .Select(d => new DoacaoResult(
                d.Id,
                d.Valor,
                d.DataDoacao,
                d.Doador.NomeCompleto,
                d.ReferenciaPagamento
            )).ToList();

        return new PaginatedResult<DoacaoResult>(query.Pagina, query.TamanhoPagina, doacoes.Count(), resultado);
    }
}