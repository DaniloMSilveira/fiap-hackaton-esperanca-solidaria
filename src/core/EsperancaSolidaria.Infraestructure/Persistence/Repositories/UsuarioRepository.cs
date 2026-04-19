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

    public async Task<bool> ExisteAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email.Value.Contains(email));
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.Value == email);
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
}