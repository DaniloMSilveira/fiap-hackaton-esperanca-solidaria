using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Autenticacao.Inputs;

public class LoginCommand : Command
{
    public string Email { get; private set; } = string.Empty;
    public string Senha { get; private set; } = string.Empty;

    public LoginCommand(string email, string senha)
    {
        Email = email;
        Senha = senha;
    }

    protected override IValidator GetValidator()
    {
        return new LoginCommandValidator();
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória");
    }
}