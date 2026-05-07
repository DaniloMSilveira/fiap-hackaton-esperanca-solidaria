namespace EsperancaSolidaria.BuildingBlocks.Events;

/// <summary>
/// Contrato para serviço de publicação e persistência de eventos de domínio.
/// Responsável por orquestrar a persistência em Event Store e publicação em Message Bus.
/// </summary>
public interface IDomainEventService
{
    Task PublishAndPersistAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
