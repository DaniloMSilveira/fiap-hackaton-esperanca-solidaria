using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infraestructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EsperancaSolidaria.Infraestructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly EsperancaSolidariaDbContext _context;

    public UsuarioRepository(EsperancaSolidariaDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterEntidadePorIdAsync(Guid id)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> ExisteAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email.Value.Contains(email));
    }

    public void Adicionar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
    }

    public void Atualizar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
    }

    public void Remover(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
    }
}