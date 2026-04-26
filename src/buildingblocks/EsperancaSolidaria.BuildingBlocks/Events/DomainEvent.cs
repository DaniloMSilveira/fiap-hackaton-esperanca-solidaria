namespace EsperancaSolidaria.BuildingBlocks.Events;

public class DomainEvent<TData> : IDomainEvent<TData>
{
    public Guid EventId { get; set; }
    public DateTime Timestamp { get; set; }
    public string EventName { get; set; }
    public string QueueName { get; set; }
    public string? CorrelationId { get; set; }
    public TData Data { get; set; }

    public DomainEvent() { }

    public DomainEvent(TData data, string eventName, string queueName, string? correlationId = null)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.Now;
        EventName = eventName;
        QueueName = queueName;
        CorrelationId = correlationId;
        Data = data;
    }

    public object GetData() => Data!;
}