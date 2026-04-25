using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Enums;

namespace EsperancaSolidaria.Domain.Interfaces.Repositories;

public interface ICampanhaRepository : IRepository<Campanha>
{
    Task<IEnumerable<Campanha>> ObterPorStatusAsync(EStatusCampanha status);
}
