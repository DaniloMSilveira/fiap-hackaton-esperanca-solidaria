namespace EsperancaSolidaria.Domain.Interfaces;

public interface IEntity
{
    Guid Id { get; }
    public DateTime DataCriacao { get; }
    public DateTime? DataAtualizacao { get; }
}