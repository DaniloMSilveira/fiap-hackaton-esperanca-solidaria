using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Campanhas.Inputs;

public class EditarCampanhaCommand : Command
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataFim { get; private set; }
    public decimal MetaFinanceira { get; private set; }
    public string Usuario { get; private set; } = string.Empty;

    public EditarCampanhaCommand(string titulo, string descricao, DateTime dataFim, decimal metaFinanceira)
    {
        Titulo = titulo;
        Descricao = descricao;
        DataFim = dataFim;
        MetaFinanceira = metaFinanceira;
    }

    public void PreencherDadosComplementares(Guid id, string usuario)
    {
        Id = id;
        Usuario = usuario;
    }

    protected override IValidator GetValidator()
    {
        return new EditarCampanhaCommandValidator();
    }
}

public class EditarCampanhaCommandValidator : AbstractValidator<EditarCampanhaCommand>
{
    public EditarCampanhaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id da campanha é obrigatório.");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória.");

        RuleFor(x => x.DataFim)
            .NotEmpty().WithMessage("Data de término é obrigatória.");

        RuleFor(x => x.MetaFinanceira)
            .GreaterThan(0).WithMessage("Meta financeira deve ser maior que zero.");

        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("Usuário é obrigatório.");
    }
}