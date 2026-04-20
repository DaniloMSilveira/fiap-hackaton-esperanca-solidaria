using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.Helpers;
using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Autenticacao.Inputs;

public class AlterarPerfilUsuarioCommand : Command
{
    public string Email { get; private set; } = string.Empty;
    public EPerfilAcesso PerfilAcesso { get; private set; }
    public string Usuario { get; private set; } = string.Empty;

    public AlterarPerfilUsuarioCommand(string email, EPerfilAcesso perfilAcesso)
    {
        Email = email;
        PerfilAcesso = perfilAcesso;
    }

    public void PreencherUsuario(string usuario)
    {
        Usuario = usuario;
    }

    protected override IValidator GetValidator()
    {
        return new AlterarPerfilUsuarioCommandValidator();
    }
}

public class AlterarPerfilUsuarioCommandValidator : AbstractValidator<AlterarPerfilUsuarioCommand>
{
    public AlterarPerfilUsuarioCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.PerfilAcesso)
            .IsInEnum().WithMessage("PerfilAcesso inválido");
    }
}