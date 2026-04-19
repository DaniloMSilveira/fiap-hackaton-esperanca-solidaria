using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EsperancaSolidaria.Infraestructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly EsperancaSolidariaDbContext _dbContext;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(EsperancaSolidariaDbContext dbContext, ILogger<UnitOfWork> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
       try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar alterações no banco de dados.");
            return (false, "Erro ao salvar alterações no banco de dados.");
        }
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}