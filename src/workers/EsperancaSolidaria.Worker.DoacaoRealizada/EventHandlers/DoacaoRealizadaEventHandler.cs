using EsperancaSolidaria.BuildingBlocks.Events;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Events;
using EsperancaSolidaria.Domain.Interfaces.Repositories;

namespace EsperancaSolidaria.Worker.DoacaoRealizada.EventHandlers;

public class DoacaoRealizadaEventHandler : IDomainEventHandler<DoacaoRealizadaEvent>
{
    private readonly ICampanhaRepository _campanhaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DoacaoRealizadaEventHandler> _logger;

    public DoacaoRealizadaEventHandler(
        ICampanhaRepository campanhaRepository,
        IUnitOfWork unitOfWork,
        ILogger<DoacaoRealizadaEventHandler> logger)
    {
        _campanhaRepository = campanhaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(DoacaoRealizadaEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processando doação realizada: {EventId}", domainEvent.EventId);

        var campanha = await _campanhaRepository.ObterPorIdAsync(domainEvent.Data.CampanhaId, cancellationToken);
        if (campanha is null)
        {
            _logger.LogError("Campanha com ID {CampanhaId} não encontrada.", domainEvent.Data.CampanhaId);
            throw new InvalidOperationException($"Campanha com ID {domainEvent.Data.CampanhaId} não encontrada.");
        }

        campanha.AdicionarDoacao(domainEvent.Data.Valor);
        _campanhaRepository.Alterar(campanha);

        var (isSuccess, errorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isSuccess)
        {
            _logger.LogError("Erro ao salvar alterações no banco de dados: {ErrorMessage}", errorMessage);
            throw new InvalidOperationException($"Erro ao salvar alterações: {errorMessage}");
        }

        _logger.LogInformation(
            "Doação processada com sucesso. Campanha: {CampanhaId}, Valor: {Valor}, Novo Total: {ValorArrecadado}",
            domainEvent.Data.CampanhaId,
            domainEvent.Data.Valor,
            campanha.ValorArrecadado);
    }
}
