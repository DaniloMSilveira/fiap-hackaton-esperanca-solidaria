using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.Domain.Helpers;
using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Autenticacao.Inputs;

public class RegistrarUsuarioCommand : Command
{
    public string NomeCompleto { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Senha { get; private set; } = string.Empty;
    public string ConfirmacaoSenha { get; private set; } = string.Empty;

    public RegistrarUsuarioCommand(string nomeCompleto, string email, string cpf, string senha, string confirmacaoSenha)
    {
        NomeCompleto = nomeCompleto;
        Email = email;
        Cpf = cpf;
        Senha = senha;
        ConfirmacaoSenha = confirmacaoSenha;
    }

    protected override IValidator GetValidator()
    {
        return new RegistrarUsuarioCommandValidator();
    }
}

public class RegistrarUsuarioCommandValidator : AbstractValidator<RegistrarUsuarioCommand>
{
    public RegistrarUsuarioCommandValidator()
    {
        RuleFor(x => x.NomeCompleto)
            .NotEmpty().WithMessage("NomeCompleto é obrigatório")
            .MinimumLength(2).WithMessage("NomeCompleto deve ter no mínimo 3 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Matches(@"^\d{11}$|^\d{3}\.\d{3}\.\d{3}-\d{2}$").WithMessage("CPF deve conter 11 dígitos");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .Must(c => ValidatorHelper.ValidarSenhaForte(c)).WithMessage("Senha deve conter no mínimo 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo");

        RuleFor(x => x.ConfirmacaoSenha)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória")
            .Equal(x => x.Senha).WithMessage("Confirmação de senha deve ser igual à senha");
    }
}