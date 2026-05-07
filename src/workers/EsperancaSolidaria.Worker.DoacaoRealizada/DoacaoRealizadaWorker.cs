using System.Text.Json;
using EsperancaSolidaria.BuildingBlocks.Events;
using EsperancaSolidaria.BuildingBlocks.Messaging;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Events;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infraestructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EsperancaSolidaria.Worker.DoacaoRealizada;

public class DoacaoRealizadaWorker: BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<DoacaoRealizadaWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DoacaoRealizadaWorker(
        IOptions<RabbitMqOptions> options,
        IMessageBus messageBus,
        ILogger<DoacaoRealizadaWorker> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _options = options.Value;
        _messageBus = messageBus;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ Doacao Realizada Worker iniciado.");

        // Consome mensagens e processa as doações
        await _messageBus.ConsumeAsync<DoacaoRealizadaEvent>(
            queueName: _options.QueueName,
            handler: async (evento, cancellationToken) => 
            {
                try 
                {
                    return await HandleAsync(evento, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar evento de doação realizada.");
                    return false;
                }
            },
            cancellationToken);
    }

    public async Task<bool> HandleAsync(DoacaoRealizadaEvent domainEvent, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        try
        {
            var domainEventDispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

            _logger.LogInformation("Despachando evento de doação realizada: {EventId}", domainEvent.EventId);

            // Despacha o evento para encontrar e executar os handlers registrados
            await domainEventDispatcher.DispatchAsync(domainEvent, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar doação realizada.");
            return false;
        }
    }
}
