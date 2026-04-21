using EsperancaSolidaria.Application.Commands.Usuarios.Inputs;
using EsperancaSolidaria.Application.Commands.Usuarios.Results;
using EsperancaSolidaria.Application.Security;
using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Domain.ValueObjects;

namespace EsperancaSolidaria.Application.Commands.Usuarios.Handlers;

public class UsuarioCommandHandler : IUsuarioCommandHandler
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAutenticacaoService _autenticacaoService;
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioCommandHandler(IUsuarioRepository usuarioRepository, IAutenticacaoService autenticacaoService, IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _autenticacaoService = autenticacaoService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandResult<CriarUsuarioResult>> HandleAsync(CriarUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
            return CommandResult<CriarUsuarioResult>.Fail(commandValidation);

        var existeUsuario = await _usuarioRepository.ExisteAsync(command.Email);
        if (existeUsuario)
            return CommandResult<CriarUsuarioResult>.Fail("Já existe um usuário cadastrado com este e-mail.");

        var email = new Email(command.Email);
        var cpf = new Cpf(command.Cpf);
        var senhaCriptografada = _autenticacaoService.CriptografarSenha(command.Senha);

        var usuario = new Usuario(command.NomeCompleto, email, cpf, senhaCriptografada, EPerfilAcesso.Doador, email.Value);
        _usuarioRepository.Adicionar(usuario);
        
        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
            return CommandResult<CriarUsuarioResult>.Fail($"Ocorreu um erro ao registrar o usuário: {commitErrorMessage}");

        var result = new CriarUsuarioResult
        {
            Id = usuario.Id,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email.Value,
            Cpf = usuario.Cpf.Value,
            DataCriacao = usuario.DataCriacao,
            UsuarioCriacao = usuario.UsuarioCriacao
        };

        return CommandResult<CriarUsuarioResult>.Success(result);
    }

    public async Task<CommandResult<EditarUsuarioResult>> HandleAsync(EditarUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
            return CommandResult<EditarUsuarioResult>.Fail(commandValidation);

        var usuario = await _usuarioRepository.ObterPorIdAsync(command.Id);
        if (usuario is null)
            return CommandResult<EditarUsuarioResult>.Fail("Usuário não encontrado.");

        var cpf = new Cpf(command.Cpf);
        usuario.AlterarDados(command.NomeCompleto, cpf, command.PerfilAcesso, command.Usuario);

        if (command.Ativo.HasValue)
        {
            if (command.Ativo == false)
                usuario.Inativar(command.Usuario);
            else
                usuario.Ativar(command.Usuario);
        }

        if (!string.IsNullOrEmpty(command.Senha))
        {
            var senhaCriptografada = _autenticacaoService.CriptografarSenha(command.Senha);
            usuario.AlterarSenha(senhaCriptografada, command.Usuario);
        }

        _usuarioRepository.Alterar(usuario);
        
        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
            return CommandResult<EditarUsuarioResult>.Fail($"Ocorreu um erro ao editar o usuário: {commitErrorMessage}");

        var result = new EditarUsuarioResult
        {
            Id = usuario.Id,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email.Value,
            Cpf = usuario.Cpf.Value,
            PerfilAcesso = usuario.PerfilAcesso,
            Ativo = usuario.Ativo,
            DataCriacao = usuario.DataCriacao,
            UsuarioCriacao = usuario.UsuarioCriacao,
            DataAtualizacao = usuario.DataAtualizacao,
            UsuarioAtualizacao = usuario.UsuarioAtualizacao
        };

        return CommandResult<EditarUsuarioResult>.Success(result);
    }

    public async Task<CommandResult> HandleAsync(RemoverUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        var commandValidation = command.Validate();
        if (!commandValidation.IsValid)
            return CommandResult.Fail(commandValidation);

        var usuario = await _usuarioRepository.ObterPorIdAsync(command.Id);
        if (usuario is null)
            return CommandResult.Fail("Usuário não encontrado.");

        _usuarioRepository.Remover(usuario);

        var (isCommited, commitErrorMessage) = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!isCommited)
            return CommandResult.Fail($"Ocorreu um erro ao remover o usuário: {commitErrorMessage}");

        return CommandResult.Success();
    }
}