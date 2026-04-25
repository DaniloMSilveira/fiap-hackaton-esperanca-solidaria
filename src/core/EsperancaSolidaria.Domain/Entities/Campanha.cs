using EsperancaSolidaria.BuildingBlocks.Domain;
using EsperancaSolidaria.Domain.Enums;

namespace EsperancaSolidaria.Domain.Entities;

public class Campanha : Entity, IAggregateRoot
{
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
    public decimal MetaFinanceira { get; private set; }
    public decimal ValorArrecadado { get; private set; }
    public EStatusCampanha Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public string UsuarioCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }
    public string? UsuarioAtualizacao { get; private set; }

    protected Campanha() { }

    public Campanha(string titulo, string descricao, DateTime dataInicio, DateTime dataFim, decimal metaFinanceira, string usuarioCriacao)
    {
        if (dataFim <= DateTime.Now)
            throw new ArgumentException("Data de término da campanha não pode ser no passado.", nameof(dataFim));

        if (metaFinanceira <= 0)
            throw new ArgumentException("Meta financeira deve ser maior que zero.", nameof(metaFinanceira));

        if (dataFim <= dataInicio)
            throw new ArgumentException("Data de término deve ser posterior à data de início.", nameof(dataFim));

        Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
        Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
        DataInicio = dataInicio;
        DataFim = dataFim;
        MetaFinanceira = metaFinanceira;
        ValorArrecadado = 0;
        Status = EStatusCampanha.Ativa;
        DataCriacao = DateTime.Now;
        UsuarioCriacao = usuarioCriacao ?? throw new ArgumentNullException(nameof(usuarioCriacao));
    }

    public void AlterarDados(string titulo, string descricao, DateTime dataFim, decimal metaFinanceira, string usuario)
    {
        if (dataFim <= DataInicio)
            throw new ArgumentException("Data de término deve ser posterior à data de início.", nameof(dataFim));

        if (metaFinanceira <= 0)
            throw new ArgumentException("Meta financeira deve ser maior que zero.", nameof(metaFinanceira));

        Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
        Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
        DataFim = dataFim;
        MetaFinanceira = metaFinanceira;
        DataAtualizacao = DateTime.Now;
        UsuarioAtualizacao = usuario;
    }

    public void AlterarStatus(EStatusCampanha status, string usuario)
    {
        Status = status;
        DataAtualizacao = DateTime.Now;
        UsuarioAtualizacao = usuario;
    }

    public void AdicionarDoacao(decimal valor)
    {
        if (Status != EStatusCampanha.Ativa)
            throw new InvalidOperationException("Não é possível adicionar doações em campanhas que não estão ativas.");

        if (valor <= 0)
            throw new ArgumentException("Valor da doação deve ser maior que zero.", nameof(valor));

        ValorArrecadado += valor;
    }
}
