using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EsperancaSolidaria.Infraestructure.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly EsperancaSolidariaDbContext _context;

    public UsuarioRepository(EsperancaSolidariaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExisteAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email.Value.Contains(email), cancellationToken);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
    }

    public async Task<IEnumerable<Usuario>> ConsultarUsuariosAsync(string? nome, string? email, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => (string.IsNullOrEmpty(nome) || u.NomeCompleto.Contains(nome)) 
                && (string.IsNullOrEmpty(email) || u.Email.Value.Contains(email)))
            .ToListAsync(cancellationToken);
    }

    #region Default

    public async Task<IEnumerable<Usuario>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios.ToListAsync(cancellationToken);
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
    }

    public void Adicionar(Usuario entity)
    {
        _context.Usuarios.Add(entity);
    }

    public void Alterar(Usuario entity)
    {
        _context.Usuarios.Update(entity);
    }

    public void Remover(Usuario entity)
    {
        _context.Usuarios.Remove(entity);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #endregion
}