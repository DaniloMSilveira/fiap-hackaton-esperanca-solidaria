namespace EsperancaSolidaria.Application.DTO.Outputs;

public class PaginacaoOutput<T>
{
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
    public int TamanhoPagina { get; set; }
    public List<T> Dados { get; set; } = new();
}