using EsperancaSolidaria.Domain.Helpers;
using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Autenticacao.Inputs;

public class AlterarSenhaCommand : Command
{
    public string Usuario { get; private set; } = string.Empty;
    public string SenhaAtual { get; private set; } = string.Empty;
    public string NovaSenha { get; private set; } = string.Empty;
    public string ConfirmacaoNovaSenha { get; private set; } = string.Empty;

    public AlterarSenhaCommand(string senhaAtual, string novaSenha, string confirmacaoNovaSenha)
    {
        SenhaAtual = senhaAtual;
        NovaSenha = novaSenha;
        ConfirmacaoNovaSenha = confirmacaoNovaSenha;
    }

    public void PreencherUsuario(string usuario)
    {
        Usuario = usuario;
    }

    protected override IValidator GetValidator()
    {
        return new AlterarSenhaCommandValidator();
    }
}

public class AlterarSenhaCommandValidator : AbstractValidator<AlterarSenhaCommand>
{
    public AlterarSenhaCommandValidator()
    {
        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("Usuário é obrigatório")
            .EmailAddress().WithMessage("Usuário deve ser um email válido");

        RuleFor(x => x.SenhaAtual)
            .NotEmpty().WithMessage("SenhaAtual é obrigatória");

        RuleFor(x => x.NovaSenha)
            .NotEmpty().WithMessage("NovaSenha é obrigatória")
            .Must(c => ValidatorHelper.ValidarSenhaForte(c)).WithMessage("NovaSenha deve conter no mínimo 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo");

        RuleFor(x => x.ConfirmacaoNovaSenha)
            .NotEmpty().WithMessage("ConfirmacaoNovaSenha é obrigatória")
            .Equal(x => x.NovaSenha).WithMessage("ConfirmacaoNovaSenha deve ser igual à nova senha");
    }
}