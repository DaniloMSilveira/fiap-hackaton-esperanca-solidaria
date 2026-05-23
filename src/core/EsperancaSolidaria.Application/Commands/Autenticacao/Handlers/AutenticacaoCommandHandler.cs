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
using Microsoft.Extensions.Logging;

namespace EsperancaSolidaria.Application.Commands.Autenticacao.Handlers;

public class AutenticacaoCommandHandler : IAutenticacaoCommandHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAutenticacaoService _autenticacaoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AutenticacaoCommandHandler> _logger;

    public AutenticacaoCommandHandler(IUsuarioRepository usuarioRepository, IAutenticacaoService autenticacaoService, IUnitOfWork unitOfWork, ILogger<AutenticacaoCommandHandler> logger)
    {
        _usuarioRepository = usuarioRepository;
        _autenticacaoService = autenticacaoService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CommandResult<RegistrarUsuarioResult>> HandleAsync(RegistrarUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando RegistrarUsuarioCommand: {Errors}", commandValidation.Errors);
            return CommandResult<RegistrarUsuarioResult>.Fail(commandValidation);
        }

        var existeUsuario = await _usuarioRepository.ExisteAsync(command.Email, cancellationToken);
        if (existeUsuario)
        {
            _logger.LogWarning("Tentativa de registrar usuário com email já existente: {Email}", command.Email);
            return CommandResult<RegistrarUsuarioResult>.Fail("Já existe um usuário cadastrado com este e-mail.");
        }

        var email = new Email(command.Email);
        var cpf = new Cpf(command.Cpf);
        var senhaCriptografada = _autenticacaoService.CriptografarSenha(command.Senha);

        var usuario = new Usuario(command.NomeCompleto, email, cpf, senhaCriptografada, EPerfilAcesso.Doador, email.Value);
        _usuarioRepository.Adicionar(usuario);
        
        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
        {
            _logger.LogError("Ocorreu um erro ao registrar o usuário: {ErrorMessage}", commitErrorMessage);
            return CommandResult<RegistrarUsuarioResult>.Fail($"Ocorreu um erro ao registrar o usuário: {commitErrorMessage}");
        }

        var result = new RegistrarUsuarioResult
        {
            Id = usuario.Id,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email.Value,
            Cpf = usuario.Cpf.Value,
            DataCriacao = usuario.DataCriacao,
        };

        _logger.LogInformation("Usuário registrado com sucesso: {Email}", usuario.Email.Value);
        return CommandResult<RegistrarUsuarioResult>.Success(result);
    }

    public async Task<CommandResult<LoginResult>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando LoginCommand: {Errors}", commandValidation.Errors);
            return CommandResult<LoginResult>.Fail(commandValidation);
        }

        var usuario = await _usuarioRepository.ObterPorEmailAsync(command.Email, cancellationToken);
        if (usuario is null || !_autenticacaoService.VerificarSenha(command.Senha, usuario.SenhaCriptografada))
        {
            _logger.LogWarning("Falha na autenticação para o email: {Email}", command.Email);
            return CommandResult<LoginResult>.Fail("Email ou senha inválidos");
        }

        var token = _autenticacaoService.GerarToken(usuario.Id, usuario.NomeCompleto, usuario.Email.Value, usuario.PerfilAcesso.GetDescription());
        var result = new LoginResult
        {
            Token = token
        };

        _logger.LogInformation("Usuário logado com sucesso: {Email}", usuario.Email.Value);
        return CommandResult<LoginResult>.Success(result);
    }

    public async Task<CommandResult> HandleAsync(AlterarSenhaCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando AlterarSenhaCommand: {Errors}", commandValidation.Errors);
            return CommandResult.Fail(commandValidation);
        }

        var usuario = await _usuarioRepository.ObterPorEmailAsync(command.Usuario, cancellationToken);
        if (usuario is null)
        {
            _logger.LogWarning("Usuário não encontrado para o email: {Email}", command.Usuario);
            return CommandResult.Fail("Usuário não encontrado");
        }

        if (!_autenticacaoService.VerificarSenha(command.SenhaAtual, usuario.SenhaCriptografada))
        {
            _logger.LogWarning("Senha atual incorreta para o email: {Email}", command.Usuario);
            return CommandResult.Fail("Senha atual incorreta");
        }

        usuario.AlterarSenha(_autenticacaoService.CriptografarSenha(command.NovaSenha), command.Usuario);
        _usuarioRepository.Alterar(usuario);

        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
        {
            _logger.LogError("Ocorreu um erro ao alterar a senha: {ErrorMessage}", commitErrorMessage);
            return CommandResult.Fail($"Ocorreu um erro ao alterar a senha: {commitErrorMessage}");
        }

        _logger.LogInformation("Senha alterada com sucesso para o usuário: {Email}", usuario.Email.Value);
        return CommandResult.Success();
    }
}