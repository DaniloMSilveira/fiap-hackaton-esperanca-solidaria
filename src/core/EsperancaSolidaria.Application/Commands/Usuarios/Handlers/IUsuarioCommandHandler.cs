using EsperancaSolidaria.Application.Commands.Usuarios.Inputs;
using EsperancaSolidaria.Application.Commands.Usuarios.Results;
using EsperancaSolidaria.BuildingBlocks.Commands;

namespace EsperancaSolidaria.Application.Commands.Usuarios.Handlers;

public interface IUsuarioCommandHandler
    : ICommandHandler<CriarUsuarioCommand, CommandResult<CriarUsuarioResult>>,
        ICommandHandler<EditarUsuarioCommand, CommandResult<EditarUsuarioResult>>,
        ICommandHandler<RemoverUsuarioCommand, CommandResult>
{
    
}