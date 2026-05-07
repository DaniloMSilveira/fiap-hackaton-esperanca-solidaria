using EsperancaSolidaria.BuildingBlocks.Events;
using EsperancaSolidaria.BuildingBlocks.EventSourcing;
using EsperancaSolidaria.BuildingBlocks.Messaging;
using Microsoft.Extensions.Logging;

namespace EsperancaSolidaria.Infraestructure.Persistence.DomainEvents;

public class DomainEventService : IDomainEventService
{
    private readonly IMessageBus _messageBus;
    private readonly IEventStore _eventStore;
    private readonly ILogger<DomainEventService> _logger;

    public DomainEventService(
        IMessageBus messageBus,
        IEventStore eventStore,
        ILogger<DomainEventService> logger)
    {
        _messageBus = messageBus;
        _eventStore = eventStore;
        _logger = logger;
    }

    public async Task PublishAndPersistAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            try
            {
                // Persiste o evento no Event Store
                await _eventStore.SaveEventAsync(
                    domainEvent.EventName,
                    domainEvent.EventId,
                    domainEvent.GetData(),
                    domainEvent.CorrelationId,
                    cancellationToken);

                // Publica a mensagem no Message Bus
                await _messageBus.PublishAsync(
                    domainEvent,
                    domainEvent.QueueName,
                    cancellationToken);

                _logger.LogInformation("Evento processado com sucesso: {EventName} ({EventId})", domainEvent.EventName, domainEvent.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento {EventName}.", domainEvent.EventName);
                throw;
            }
        }
    }
}
