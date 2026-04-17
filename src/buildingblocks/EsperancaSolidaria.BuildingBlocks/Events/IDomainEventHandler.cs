namespace EsperancaSolidaria.BuildingBlocks.Events;

/// <summary>
/// Contrato para handlers de eventos de domínio.
/// </summary>
public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}