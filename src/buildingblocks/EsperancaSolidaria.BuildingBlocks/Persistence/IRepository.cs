using EsperancaSolidaria.BuildingBlocks.Domain;

namespace EsperancaSolidaria.BuildingBlocks.Persistence;

/// <summary>
/// Contrato genérico para repositórios de agregados.
/// </summary>
public interface IRepository<TEntity> : IDisposable
    where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Adicionar(TEntity entity);
    void Alterar(TEntity entity);
    void Remover(TEntity entity);
}