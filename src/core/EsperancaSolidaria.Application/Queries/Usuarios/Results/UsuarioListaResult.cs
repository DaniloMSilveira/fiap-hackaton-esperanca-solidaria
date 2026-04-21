using EsperancaSolidaria.Domain.Enums;

namespace EsperancaSolidaria.Application.Queries.Usuarios.Inputs;

/// <summary>
/// Representa um usuário no resultado da listagem
/// </summary>
public class UsuarioListaResult
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
}
