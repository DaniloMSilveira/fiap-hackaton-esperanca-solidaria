using EsperancaSolidaria.Application.Commands.Campanhas.Inputs;
using EsperancaSolidaria.Application.Commands.Campanhas.Results;
using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Interfaces.Repositories;

namespace EsperancaSolidaria.Application.Commands.Campanhas.Handlers;

public class CampanhaCommandHandler : ICampanhaCommandHandler
{
    private readonly ICampanhaRepository _campanhaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CampanhaCommandHandler(ICampanhaRepository campanhaRepository, IUnitOfWork unitOfWork)
    {
        _campanhaRepository = campanhaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandResult<CriarCampanhaResult>> HandleAsync(CriarCampanhaCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
            return CommandResult<CriarCampanhaResult>.Fail(commandValidation);

        var campanha = new Campanha(command.Titulo, command.Descricao, command.DataInicio, command.DataFim, command.MetaFinanceira, "Sistema");
        _campanhaRepository.Adicionar(campanha);
        
        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
            return CommandResult<CriarCampanhaResult>.Fail($"Ocorreu um erro ao criar a campanha: {commitErrorMessage}");

        var result = new CriarCampanhaResult
        {
            Id = campanha.Id,
            Titulo = campanha.Titulo,
            DataCriacao = campanha.DataCriacao
        };

        return CommandResult<CriarCampanhaResult>.Success(result);
    }

    public async Task<CommandResult<EditarCampanhaResult>> HandleAsync(EditarCampanhaCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
            return CommandResult<EditarCampanhaResult>.Fail(commandValidation);

        var campanha = await _campanhaRepository.ObterPorIdAsync(command.Id);
        if (campanha is null)
            return CommandResult<EditarCampanhaResult>.Fail("Campanha não encontrada.");

        campanha.AlterarDados(command.Titulo, command.Descricao, command.DataFim, command.MetaFinanceira, command.Usuario);
        _campanhaRepository.Alterar(campanha);

        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
            return CommandResult<EditarCampanhaResult>.Fail($"Ocorreu um erro ao editar a campanha: {commitErrorMessage}");

        var result = new EditarCampanhaResult
        {
            Id = campanha.Id,
            Titulo = campanha.Titulo,
            DataAtualizacao = campanha.DataAtualizacao
        };

        return CommandResult<EditarCampanhaResult>.Success(result);
    }

    public async Task<CommandResult> HandleAsync(RemoverCampanhaCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
            return CommandResult.Fail(commandValidation);

        var campanha = await _campanhaRepository.ObterPorIdAsync(command.Id);
        if (campanha is null)
            return CommandResult.Fail("Campanha não encontrada.");

        _campanhaRepository.Remover(campanha);

        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
            return CommandResult.Fail($"Ocorreu um erro ao remover a campanha: {commitErrorMessage}");

        return CommandResult.Success();
    }

    public async Task<CommandResult> HandleAsync(AlterarStatusCampanhaCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
            return CommandResult.Fail(commandValidation);

        var campanha = await _campanhaRepository.ObterPorIdAsync(command.Id);
        if (campanha is null)
            return CommandResult.Fail("Campanha não encontrada.");

        campanha.AlterarStatus(command.Status, command.Usuario);
        _campanhaRepository.Alterar(campanha);

        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
            return CommandResult.Fail($"Ocorreu um erro ao alterar o status da campanha: {commitErrorMessage}");

        return CommandResult.Success();
    }
}
