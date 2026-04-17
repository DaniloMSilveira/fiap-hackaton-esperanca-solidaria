using EsperancaSolidaria.BuildingBlocks.Events;

namespace EsperancaSolidaria.BuildingBlocks.Domain;

/// <summary>
/// Classe generica para representar uma entidade de domínio
/// </summary>
public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
        /// Adiciona um evento de domínio à entidade.
    /// </summary>
    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
        /// Limpa todos os eventos de domínio acumulados.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}