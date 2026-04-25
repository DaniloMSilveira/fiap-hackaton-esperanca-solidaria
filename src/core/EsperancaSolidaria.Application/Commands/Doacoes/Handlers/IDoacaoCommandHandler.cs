using EsperancaSolidaria.Application.Commands.Doacoes.Inputs;
using EsperancaSolidaria.Application.Commands.Doacoes.Results;
using EsperancaSolidaria.BuildingBlocks.Commands;

namespace EsperancaSolidaria.Application.Commands.Doacoes.Handlers;

public interface IDoacaoCommandHandler
    : ICommandHandler<CriarDoacaoCommand, CommandResult<CriarDoacaoResult>>
{
}
