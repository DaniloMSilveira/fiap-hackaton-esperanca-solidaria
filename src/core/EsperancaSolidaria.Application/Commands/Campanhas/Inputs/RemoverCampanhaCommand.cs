using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Campanhas.Inputs;

public class RemoverCampanhaCommand : Command
{
    public Guid Id { get; private set; }
    public string Usuario { get; private set; } = string.Empty;

    public RemoverCampanhaCommand(Guid id, string usuario)
    {
        Id = id;
        Usuario = usuario;
    }

    protected override IValidator GetValidator()
    {
        return new RemoverCampanhaCommandValidator();
    }
}

public class RemoverCampanhaCommandValidator : AbstractValidator<RemoverCampanhaCommand>
{
    public RemoverCampanhaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id da campanha é obrigatório.");

        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("Usuário é obrigatório.");
    }
}