using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Entities;

namespace EsperancaSolidaria.Domain.Interfaces.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<bool> ExisteAsync(string email);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<IEnumerable<Usuario>> ConsultarUsuariosAsync(string? nome, string? email, CancellationToken cancellationToken = default);
}