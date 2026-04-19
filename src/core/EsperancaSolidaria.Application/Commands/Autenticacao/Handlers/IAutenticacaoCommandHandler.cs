using EsperancaSolidaria.Application.Commands.Autenticacao.Inputs;
using EsperancaSolidaria.Application.Commands.Autenticacao.Results;
using EsperancaSolidaria.BuildingBlocks.Commands;

namespace EsperancaSolidaria.Application.Commands.Autenticacao.Handlers;

public interface IAutenticacaoCommandHandler 
    : ICommandHandler<RegistrarUsuarioCommand, CommandResult<RegistrarUsuarioCommandResult>>,
        ICommandHandler<LoginCommand, CommandResult<LoginCommandResult>>
{
    
}