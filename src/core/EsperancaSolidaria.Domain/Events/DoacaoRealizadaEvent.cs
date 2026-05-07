using System.Text.Json.Serialization;
using EsperancaSolidaria.BuildingBlocks.Events;

namespace EsperancaSolidaria.Domain.Events;

public record DoacaoRealizadaData(Guid DoacaoId, Guid CampanhaId, Guid DoadorId, decimal Valor, string ReferenciaPagamento);

public class DoacaoRealizadaEvent : DomainEvent<DoacaoRealizadaData>
{
    public DoacaoRealizadaEvent() { }
    
    public DoacaoRealizadaEvent(DoacaoRealizadaData data, string? correlationId = null)
        : base(data, nameof(DoacaoRealizadaEvent), "esperanca_solidaria_doacao_realizada", correlationId)
    {
    }
}
