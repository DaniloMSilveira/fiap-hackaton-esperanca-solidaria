using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Entities;

namespace EsperancaSolidaria.Domain.Interfaces.Repositories;

public interface IDoacaoRepository : IRepository<Doacao>
{
    Task<IEnumerable<Doacao>> ObterDoacoesAsync(Guid campanhaId);
    Task<decimal> ObterSomaValorPorCampanhaAsync(Guid campanhaId);
    Task<bool> ExisteDoacaoAsync(string referenciaPagamento);
}