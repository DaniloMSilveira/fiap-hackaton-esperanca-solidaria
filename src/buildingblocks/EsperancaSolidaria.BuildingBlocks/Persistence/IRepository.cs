using EsperancaSolidaria.BuildingBlocks.Domain;

namespace EsperancaSolidaria.BuildingBlocks.Persistence;

/// <summary>
/// Contrato genérico para repositórios de agregados.
/// </summary>
public interface IRepository<TAggregateRoot> : IDisposable
    where TAggregateRoot : IAggregateRoot
{
    Task<IEnumerable<TAggregateRoot>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<TAggregateRoot?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Adicionar(TAggregateRoot entity);
    void Alterar(TAggregateRoot entity);
    void Remover(TAggregateRoot entity);
}