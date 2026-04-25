using EsperancaSolidaria.Domain.Enums;
using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Campanhas.Inputs;

public class AlterarStatusCampanhaCommand : Command
{
    public Guid Id { get; private set; }
    public EStatusCampanha Status { get; private set; }
    public string Usuario { get; private set; } = string.Empty;

    public AlterarStatusCampanhaCommand(EStatusCampanha status)
    {
        Status = status;
    }

    public void PreencherDadosComplementares(Guid id, string usuario)
    {
        Id = id;
        Usuario = usuario;
    }

    protected override IValidator GetValidator()
    {
        return new AlterarStatusCampanhaCommandValidator();
    }
}

public class AlterarStatusCampanhaCommandValidator : AbstractValidator<AlterarStatusCampanhaCommand>
{
    public AlterarStatusCampanhaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id da campanha é obrigatório.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status da campanha inválido.");

        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("Usuário é obrigatório.");
    }
}