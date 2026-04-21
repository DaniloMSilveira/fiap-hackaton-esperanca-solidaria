using System.Data;
using EsperancaSolidaria.BuildingBlocks.Validators;
using EsperancaSolidaria.Domain.Enums;
using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Usuarios.Inputs;

public class EditarUsuarioCommand : Command
{
    public Guid Id { get; private set; }
    public string NomeCompleto { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public EPerfilAcesso PerfilAcesso { get; private set; }
    public string? Senha { get; private set; } = string.Empty;
    public bool? Ativo { get; private set; }
    public string Usuario { get; private set; } = string.Empty;

    public EditarUsuarioCommand(
        string nomeCompleto,
        string cpf,
        EPerfilAcesso perfilAcesso,
        string? senha,
        bool? ativo
    )
    {
        NomeCompleto = nomeCompleto;
        Cpf = cpf;
        PerfilAcesso = perfilAcesso;
        Senha = senha;
        Ativo = ativo;
    }

    public void PreencherDadosComplementares(Guid id, string usuario)
    {
        Id = id;
        Usuario = usuario;
    }

    protected override IValidator GetValidator()
    {
        return new EditarUsuarioCommandValidator();
    }
}

public class EditarUsuarioCommandValidator : AbstractValidator<EditarUsuarioCommand>
{
    public EditarUsuarioCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório")
            .Must(id => id != Guid.Empty).WithMessage("Id inválido");

        RuleFor(x => x.NomeCompleto)
            .NotEmpty().WithMessage("NomeCompleto é obrigatório")
            .MinimumLength(2).WithMessage("NomeCompleto deve ter no mínimo 3 caracteres");

        RuleFor(x => x.Cpf)
            .Must(c => CPFValidator.IsValidCpf(c))
            .WithMessage("CPF inválido")
            .When(c => !string.IsNullOrEmpty(c.Cpf));

        RuleFor(x => x.Senha)
            .Must(c => PasswordValidator.StrongPasswordValidate(c))
            .WithMessage("Senha deve conter no mínimo 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo")
            .When(c => !string.IsNullOrEmpty(c.Senha));

        RuleFor(x => x.PerfilAcesso)
            .IsInEnum()
            .WithMessage("Perfil de acesso inválido");

        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("Usuário é obrigatório");
    }
}