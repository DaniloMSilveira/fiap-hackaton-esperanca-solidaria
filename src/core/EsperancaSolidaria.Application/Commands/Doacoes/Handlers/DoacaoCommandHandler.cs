using EsperancaSolidaria.Application.Commands.Doacoes.Inputs;
using EsperancaSolidaria.Application.Commands.Doacoes.Results;
using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.Events;
using EsperancaSolidaria.Domain.Interfaces.Repositories;

namespace EsperancaSolidaria.Application.Commands.Doacoes.Handlers;

public class DoacaoCommandHandler : IDoacaoCommandHandler
{
    private readonly IDoacaoRepository _doacaoRepository;
    private readonly ICampanhaRepository _campanhaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DoacaoCommandHandler(IDoacaoRepository doacaoRepository, ICampanhaRepository campanhaRepository, IUnitOfWork unitOfWork)
    {
        _doacaoRepository = doacaoRepository;
        _campanhaRepository = campanhaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandResult<CriarDoacaoResult>> HandleAsync(CriarDoacaoCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
            return CommandResult<CriarDoacaoResult>.Fail(commandValidation);

        var campanha = await _campanhaRepository.ObterPorIdAsync(command.CampanhaId);
        if (campanha is null)
            return CommandResult<CriarDoacaoResult>.Fail("Campanha não encontrada.");

        if (campanha.Status != EStatusCampanha.Ativa)
            return CommandResult<CriarDoacaoResult>.Fail("Não é possível fazer doações em campanhas que não estão ativas.");

        var doacaoExistente = await _doacaoRepository.ExisteDoacaoAsync(command.ReferenciaPagamento);
        if (doacaoExistente)
            return CommandResult<CriarDoacaoResult>.Fail("Já existe uma doação registrada com a mesma referência de pagamento.");

        var doacao = new Doacao(command.CampanhaId, command.DoadorId, command.Valor, command.ReferenciaPagamento);
        _doacaoRepository.Adicionar(doacao);

        var eventoDoacao = new DoacaoRealizadaEvent(doacao.Id, command.CampanhaId, command.DoadorId, command.ReferenciaPagamento, command.Valor);
        campanha.AddDomainEvent(eventoDoacao);

        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
            return CommandResult<CriarDoacaoResult>.Fail($"Ocorreu um erro ao registrar a doação: {commitErrorMessage}");

        var result = new CriarDoacaoResult
        {
            Id = doacao.Id,
            CampanhaId = doacao.CampanhaId,
            DoadorId = doacao.DoadorId,
            Valor = doacao.Valor,
            DataDoacao = doacao.DataDoacao
        };

        return CommandResult<CriarDoacaoResult>.Success(result);
    }
}
