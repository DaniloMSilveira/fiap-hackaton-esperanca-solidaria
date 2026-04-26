namespace EsperancaSolidaria.BuildingBlocks.Events;

/// <summary>
/// Contrato que marca um evento de domínio
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime Timestamp { get; }
    string EventName { get; }
    string QueueName { get; }
    string? CorrelationId { get; }

    object GetData();
}

/// <summary>
/// Contrato que marca um evento de domínio com tipo de dado específico
/// </summary>
public interface IDomainEvent<TData> : IDomainEvent
{
    TData Data { get; }
}