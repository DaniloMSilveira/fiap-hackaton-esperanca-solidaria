using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EsperancaSolidaria.Infraestructure.Persistence.Repositories;

public class DoacaoRepository : IDoacaoRepository
{
    private readonly EsperancaSolidariaDbContext _context;

    public DoacaoRepository(EsperancaSolidariaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Doacao>> ObterDoacoesAsync(Guid campanhaId)
    {
        return await _context.Doacoes
            .AsNoTracking()
            .Include(d => d.Doador)
            .Where(d => d.CampanhaId == campanhaId)
            .OrderByDescending(d => d.DataDoacao)
            .ToListAsync();
    }

    public async Task<decimal> ObterSomaValorPorCampanhaAsync(Guid campanhaId)
    {
        return await _context.Doacoes
            .Where(d => d.CampanhaId == campanhaId)
            .SumAsync(d => d.Valor);
    }

    public async Task<bool> ExisteDoacaoAsync(string referenciaPagamento)
    {
        return await _context.Doacoes.AnyAsync(d => d.ReferenciaPagamento == referenciaPagamento);
    }


    #region Default

    public async Task<IEnumerable<Doacao>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Doacoes.ToListAsync(cancellationToken);
    }

    public async Task<Doacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Doacoes.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public void Adicionar(Doacao entity)
    {
        _context.Doacoes.Add(entity);
    }

    public void Alterar(Doacao entity)
    {
        _context.Doacoes.Update(entity);
    }

    public void Remover(Doacao entity)
    {
        _context.Doacoes.Remove(entity);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #endregion
}
