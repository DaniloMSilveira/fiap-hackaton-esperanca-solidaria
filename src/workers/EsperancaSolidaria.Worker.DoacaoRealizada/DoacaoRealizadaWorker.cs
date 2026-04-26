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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = _options.QueueName;

        _logger.LogInformation("RabbitMQ Doacao Realizada Worker iniciado.");

        // Inicializa as filas (se ainda não estiverem criadas)
        await _messageBus.InitializeQueuesAsync(stoppingToken);

        // Consome mensagens e processa as doações
        await _messageBus.ConsumeAsync($"{queueName}_queue", async messageJson =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            try
            {
                var campanhaRepository = scope.ServiceProvider.GetRequiredService<ICampanhaRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();


                var rawEvent = JsonSerializer.Deserialize<DoacaoRealizadaEvent>(
                    messageJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (rawEvent?.Data == null)
                {
                    _logger.LogWarning("Mensagem inválida: {Json}", messageJson);
                    return;
                }

                var doacaoData = rawEvent.Data;
                if (doacaoData == null)
                {
                    _logger.LogWarning("Mensagem inválida: {Json}", messageJson);
                    return;
                }

                _logger.LogInformation("Processando doação realizada: {MessageId}", rawEvent.EventId);

                var campanha = await campanhaRepository.ObterPorIdAsync(doacaoData.CampanhaId);

                if (campanha == null)
                {
                    _logger.LogError("Campanha com ID {CampanhaId} não encontrada.", doacaoData.CampanhaId);
                    return;
                }

                campanha.AdicionarDoacao(doacaoData.Valor);
                campanhaRepository.Alterar(campanha);

                var (isSuccess, errorMessage) = await unitOfWork.SaveChangesAsync();
                if (!isSuccess)
                {
                    _logger.LogError("Erro ao salvar alterações no banco de dados: {ErrorMessage}", errorMessage);
                    return;
                }
                
                _logger.LogInformation(
                    "Doação processada com sucesso. Campanha: {CampanhaId}, Valor: {Valor}, Novo Total: {ValorArrecadado}",
                    doacaoData.CampanhaId,
                    doacaoData.Valor,
                    campanha.ValorArrecadado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar doação realizada.");
            }
        }, stoppingToken);
    }
}
