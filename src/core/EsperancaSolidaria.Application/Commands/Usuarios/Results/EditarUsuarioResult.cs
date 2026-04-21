using EsperancaSolidaria.Domain.Enums;

namespace EsperancaSolidaria.Application.Commands.Usuarios.Results;

public class EditarUsuarioResult
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public EPerfilAcesso PerfilAcesso { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public string UsuarioCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public string UsuarioAtualizacao { get; set; }
}