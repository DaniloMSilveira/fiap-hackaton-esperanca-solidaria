namespace EsperancaSolidaria.BuildingBlocks.EventSourcing;

public class EventStoreDocument
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public object Data { get; set; } = new object();
    public string? CorrelationId { get; set; }
    public DateTime Timestamp { get; set; }
}