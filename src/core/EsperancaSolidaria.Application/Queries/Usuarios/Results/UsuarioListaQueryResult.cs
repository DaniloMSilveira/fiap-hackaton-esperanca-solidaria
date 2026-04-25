using EsperancaSolidaria.Domain.Enums;

namespace EsperancaSolidaria.Application.Queries.Usuarios.Results;

/// <summary>
/// Representa um usuário no resultado da listagem
/// </summary>
public class UsuarioListaQueryResult
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
}
