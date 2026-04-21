using EsperancaSolidaria.BuildingBlocks.Validators;
using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Usuarios.Inputs;

public class CriarUsuarioCommand : Command
{
    public string NomeCompleto { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Senha { get; private set; } = string.Empty;
    public string ConfirmacaoSenha { get; private set; } = string.Empty;
    public string Usuario { get; private set; } = string.Empty;

    public CriarUsuarioCommand(string nomeCompleto, string email, string cpf, string senha, string confirmacaoSenha)
    {
        NomeCompleto = nomeCompleto;
        Email = email;
        Cpf = cpf;
        Senha = senha;
        ConfirmacaoSenha = confirmacaoSenha;
    }

    public void PreencherUsuario(string usuario)
    {
        Usuario = usuario;
    }

    protected override IValidator GetValidator()
    {
        return new CriarUsuarioCommandValidator();
    }
}

public class CriarUsuarioCommandValidator : AbstractValidator<CriarUsuarioCommand>
{
    public CriarUsuarioCommandValidator()
    {
        RuleFor(x => x.NomeCompleto)
            .NotEmpty().WithMessage("NomeCompleto é obrigatório")
            .MinimumLength(2).WithMessage("NomeCompleto deve ter no mínimo 3 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Must(c => CPFValidator.IsValidCpf(c)).WithMessage("CPF inválido");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .Must(c => PasswordValidator.StrongPasswordValidate(c)).WithMessage("Senha deve conter no mínimo 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo");

        RuleFor(x => x.ConfirmacaoSenha)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória")
            .Equal(x => x.Senha).WithMessage("Confirmação de senha deve ser igual à senha");

        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("Usuário é obrigatório");
    }
}