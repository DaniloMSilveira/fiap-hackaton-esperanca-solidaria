namespace EsperancaSolidaria.BuildingBlocks.Events;

/// <summary>
/// Contrato que marca um evento de domínio
/// </summary>
public interface IDomainEvent
{
    DateTime Timestamp { get; }
}