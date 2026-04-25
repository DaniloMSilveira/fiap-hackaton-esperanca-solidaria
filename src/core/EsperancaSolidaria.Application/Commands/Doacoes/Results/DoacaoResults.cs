namespace EsperancaSolidaria.Application.Commands.Doacoes.Results;

public class CriarDoacaoResult
{
    public Guid Id { get; set; }
    public Guid CampanhaId { get; set; }
    public Guid DoadorId { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataDoacao { get; set; }
}
