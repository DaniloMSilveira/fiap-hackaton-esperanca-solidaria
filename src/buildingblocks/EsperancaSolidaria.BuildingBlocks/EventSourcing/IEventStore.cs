using EsperancaSolidaria.BuildingBlocks.Events;

namespace EsperancaSolidaria.BuildingBlocks.EventSourcing;

public interface IEventStore
{
    Task SaveEventAsync(string eventName, Guid eventId, object eventData, string? correlationId = null, CancellationToken cancellationToken = default);
}