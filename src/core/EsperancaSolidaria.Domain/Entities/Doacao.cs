namespace EsperancaSolidaria.Domain.Entities;

public class Doacao : Entity
{
    public Guid CampanhaId { get; private set; }
    public virtual Campanha Campanha { get; private set; }
    public Guid DoadorId { get; private set; }
    public virtual Usuario Doador { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime DataDoacao { get; private set; }
    public string ReferenciaPagamento { get; private set; }
    public DateTime DataCriacao { get; private set; }

    protected Doacao() { }

    public Doacao(Guid campanhaId, Guid doadorId, decimal valor, string referenciaPagamento)
    {
        if (campanhaId == Guid.Empty)
            throw new ArgumentException("CampanhaId não pode estar vazio.", nameof(campanhaId));

        if (doadorId == Guid.Empty)
            throw new ArgumentException("DoadorId não pode estar vazio.", nameof(doadorId));

        if (string.IsNullOrEmpty(referenciaPagamento))
            throw new ArgumentException("Referência do pagamento não pode estar vazio.", nameof(referenciaPagamento));

        if (valor <= 0)
            throw new ArgumentException("Valor da doação deve ser maior que zero.", nameof(valor));

        CampanhaId = campanhaId;
        DoadorId = doadorId;
        Valor = valor;
        DataDoacao = DateTime.Now;
        ReferenciaPagamento = referenciaPagamento;
        DataCriacao = DateTime.Now;
    }
}
