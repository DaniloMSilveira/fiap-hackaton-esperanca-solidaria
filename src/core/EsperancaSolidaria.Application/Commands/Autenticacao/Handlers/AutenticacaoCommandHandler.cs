using EsperancaSolidaria.Application.Commands.Autenticacao.Inputs;
using EsperancaSolidaria.Application.Commands.Autenticacao.Results;
using EsperancaSolidaria.Application.Security;
using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.BuildingBlocks.Extensions;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Domain.ValueObjects;

namespace EsperancaSolidaria.Application.Commands.Autenticacao.Handlers;

public class AutenticacaoCommandHandler : IAutenticacaoCommandHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAutenticacaoService _autenticacaoService;
    private readonly IUnitOfWork _unitOfWork;

    public AutenticacaoCommandHandler(IUsuarioRepository usuarioRepository, IAutenticacaoService autenticacaoService, IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _autenticacaoService = autenticacaoService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandResult<RegistrarUsuarioCommandResult>> HandleAsync(RegistrarUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
        {
            return CommandResult<RegistrarUsuarioCommandResult>.Fail(commandValidation);
        }

        var existeUsuario = await _usuarioRepository.ExisteAsync(command.Email);
        if (existeUsuario)
            return CommandResult<RegistrarUsuarioCommandResult>.Fail("Já existe um usuário cadastrado com este e-mail.");

        var email = new Email(command.Email);
        var cpf = new Cpf(command.Cpf);
        var senhaCriptografada = _autenticacaoService.CriptografarSenha(command.Senha);

        var usuario = new Usuario(command.NomeCompleto, email, cpf, senhaCriptografada, EPerfilAcesso.Doador, email.Value);
        _usuarioRepository.Adicionar(usuario);
        
        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
        {
            return CommandResult<RegistrarUsuarioCommandResult>.Fail($"Ocorreu um erro ao persistir o usuário: {commitErrorMessage}");
        }

        var result = new RegistrarUsuarioCommandResult
        {
            Id = usuario.Id,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email.Value,
            Cpf = usuario.Cpf.Value,
            DataCriacao = usuario.DataCriacao,
        };

        return CommandResult<RegistrarUsuarioCommandResult>.Success(result);
    }

    public async Task<CommandResult<LoginCommandResult>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
        {
            return CommandResult<LoginCommandResult>.Fail(commandValidation);
        }

        var user = await _usuarioRepository.ObterPorEmailAsync(command.Email);
        if (user is null || !_autenticacaoService.VerificarSenha(command.Senha, user.SenhaCriptografada))
            return CommandResult<LoginCommandResult>.Fail("Email ou senha inválidos");

        var token = _autenticacaoService.GerarToken(user.Id, user.NomeCompleto, user.Email.Value, user.PerfilAcesso.GetDescription().ToUpper());
        var result = new LoginCommandResult
        {
            Token = token
        };

        return CommandResult<LoginCommandResult>.Success(result);
    }
}