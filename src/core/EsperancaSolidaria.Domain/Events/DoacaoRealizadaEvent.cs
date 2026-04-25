using EsperancaSolidaria.BuildingBlocks.Events;

namespace EsperancaSolidaria.Domain.Events;

public class DoacaoRealizadaEvent : IDomainEvent
{
    public Guid DoacaoId { get; set; }
    public Guid CampanhaId { get; set; }
    public Guid DoadorId { get; set; }
    public decimal Valor { get; set; }
    public string ReferenciaPagamento { get; set; }
    public DateTime Timestamp { get; set; }

    public DoacaoRealizadaEvent(Guid doacaoId, Guid campanhaId, Guid doadorId, string referenciaPagamento, decimal valor)
    {
        DoacaoId = doacaoId;
        CampanhaId = campanhaId;
        DoadorId = doadorId;
        ReferenciaPagamento = referenciaPagamento;
        Valor = valor;
        Timestamp = DateTime.Now;
    }
}
