using EsperancaSolidaria.BuildingBlocks.Domain;

namespace EsperancaSolidaria.BuildingBlocks.Persistence;

/// <summary>
/// Contrato genérico para repositórios de agregados.
/// </summary>
public interface IRepository<TAggregateRoot> 
    where TAggregateRoot : IAggregateRoot
{
    Task<TAggregateRoot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);
}