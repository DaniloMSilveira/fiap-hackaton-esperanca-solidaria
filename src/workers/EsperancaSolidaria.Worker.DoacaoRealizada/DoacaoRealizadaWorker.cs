using System.Text.Json;
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
                    await HandleAsync(evento, cancellationToken);
                    return true;
                }
                catch
                {
                    return false;
                }
            },
            cancellationToken);
    }

    public async Task HandleAsync(DoacaoRealizadaEvent message, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        try
        {
            var campanhaRepository = scope.ServiceProvider.GetRequiredService<ICampanhaRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            _logger.LogInformation("Processando doação realizada: {MessageId}", message.EventId);

            var campanha = await campanhaRepository.ObterPorIdAsync(message.Data.CampanhaId, cancellationToken);

            if (campanha == null)
            {
                _logger.LogError("Campanha com ID {CampanhaId} não encontrada.", message.Data.CampanhaId);
                return;
            }

            campanha.AdicionarDoacao(message.Data.Valor);
            campanhaRepository.Alterar(campanha);

            var (isSuccess, errorMessage) = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (!isSuccess)
            {
                _logger.LogError("Erro ao salvar alterações no banco de dados: {ErrorMessage}", errorMessage);
                return;
            }
            
            _logger.LogInformation(
                "Doação processada com sucesso. Campanha: {CampanhaId}, Valor: {Valor}, Novo Total: {ValorArrecadado}",
                message.Data.CampanhaId,
                message.Data.Valor,
                campanha.ValorArrecadado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar doação realizada.");
            throw;
        }
    }
}
