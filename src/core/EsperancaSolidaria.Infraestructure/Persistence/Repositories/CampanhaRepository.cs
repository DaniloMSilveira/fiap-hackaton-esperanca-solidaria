using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EsperancaSolidaria.Infraestructure.Persistence.Repositories;

public class CampanhaRepository : ICampanhaRepository
{
    private readonly EsperancaSolidariaDbContext _context;

    public CampanhaRepository(EsperancaSolidariaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Campanha>> ObterPorStatusAsync(EStatusCampanha status)
    {
        return await _context.Campanhas
            .AsNoTracking()
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.DataCriacao)
            .ToListAsync();
    }

    #region Default

    public async Task<IEnumerable<Campanha>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Campanhas.ToListAsync();
    }

    public async Task<Campanha?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Campanhas.FirstOrDefaultAsync(c => c.Id == id);
    }

    public void Adicionar(Campanha campanha)
    {
        _context.Campanhas.Add(campanha);
    }

    public void Alterar(Campanha campanha)
    {
        _context.Campanhas.Update(campanha);
    }

    public void Remover(Campanha campanha)
    {
        _context.Campanhas.Remove(campanha);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #endregion
}
