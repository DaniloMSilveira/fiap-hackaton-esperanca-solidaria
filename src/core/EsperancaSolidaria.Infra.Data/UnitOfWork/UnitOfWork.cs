using EsperancaSolidaria.Domain.Interfaces;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infra.Data.Contexts;
using EsperancaSolidaria.Infra.Data.Repositories;

namespace EsperancaSolidaria.Infra.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly EsperancaSolidariaDbContext _context;
    private IUsuarioRepository _usuarioRepository;

    public UnitOfWork(EsperancaSolidariaDbContext context, IUsuarioRepository usuarioRepository)
    {
        _context = context;
        _usuarioRepository = usuarioRepository;
    }

    public IUsuarioRepository UsuarioRepository => _usuarioRepository;

    public async Task<bool> Commit()
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}