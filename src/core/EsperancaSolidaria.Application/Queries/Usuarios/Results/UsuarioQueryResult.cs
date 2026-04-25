using EsperancaSolidaria.Domain.Enums;

namespace EsperancaSolidaria.Application.Queries.Usuarios.Results;

/// <summary>
/// Representa os dados completos de um usuário
/// </summary>
public class UsuarioQueryResult
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public EPerfilAcesso PerfilAcessoId { get; set; }
    public string PerfilAcessoDescricao { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
}
