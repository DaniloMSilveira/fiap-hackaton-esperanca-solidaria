namespace EsperancaSolidaria.Application.Commands.Autenticacao.Results;

public class RegistrarUsuarioCommandResult
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public DateTime DataCriacao { get; set; }
}