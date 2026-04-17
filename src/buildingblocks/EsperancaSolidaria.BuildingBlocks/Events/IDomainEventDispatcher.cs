namespace EsperancaSolidaria.BuildingBlocks.Events;

/// <summary>
/// Contrato para despachar eventos de domínio.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
