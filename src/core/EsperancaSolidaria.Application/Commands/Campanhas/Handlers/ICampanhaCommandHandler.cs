using EsperancaSolidaria.Application.Commands.Campanhas.Inputs;
using EsperancaSolidaria.Application.Commands.Campanhas.Results;
using EsperancaSolidaria.BuildingBlocks.Commands;

namespace EsperancaSolidaria.Application.Commands.Campanhas.Handlers;

public interface ICampanhaCommandHandler
    : ICommandHandler<CriarCampanhaCommand, CommandResult<CriarCampanhaResult>>,
        ICommandHandler<EditarCampanhaCommand, CommandResult<EditarCampanhaResult>>,
        ICommandHandler<RemoverCampanhaCommand, CommandResult>,
        ICommandHandler<AlterarStatusCampanhaCommand, CommandResult>
{
}
