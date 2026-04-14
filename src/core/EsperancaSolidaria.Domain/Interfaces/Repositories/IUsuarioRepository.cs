using EsperancaSolidaria.Domain.Entities;

namespace EsperancaSolidaria.Domain.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterEntidadePorIdAsync(Guid id);
    Task<bool> ExisteAsync(string email);
    void Adicionar(Usuario usuario);
    void Atualizar(Usuario usuario);
    void Remover(Usuario usuario);
}