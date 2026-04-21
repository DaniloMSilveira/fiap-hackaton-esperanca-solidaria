namespace EsperancaSolidaria.Application.Commands.Usuarios.Results;

public class CriarUsuarioResult
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public DateTime DataCriacao { get; set; }
    public string UsuarioCriacao { get; set; }
}