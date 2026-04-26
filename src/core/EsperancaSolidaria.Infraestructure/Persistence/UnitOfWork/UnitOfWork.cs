using EsperancaSolidaria.BuildingBlocks.Events;
using EsperancaSolidaria.BuildingBlocks.Messaging;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Infraestructure.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace EsperancaSolidaria.Infraestructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly EsperancaSolidariaDbContext _dbContext;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(EsperancaSolidariaDbContext dbContext, IMessageBus messageBus, ILogger<UnitOfWork> logger)
    {
        _dbContext = dbContext;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
       try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            var domainEvents = _dbContext.ChangeTracker
                .Entries()
                .SelectMany(x => x.Entity is Entity entity 
                    ? entity.DomainEvents 
                    : Array.Empty<IDomainEvent>())
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                try
                {
                    await _messageBus.PublishAsync(domainEvent, domainEvent.QueueName, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao publicar evento {domainEvent.GetType().Name}.");
                }
            }

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