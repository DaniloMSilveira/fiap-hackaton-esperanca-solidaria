using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.BuildingBlocks.Validators;
using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Campanhas.Inputs;

public class CriarCampanhaCommand : Command
{
    public string Titulo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
    public decimal MetaFinanceira { get; private set; }

    public CriarCampanhaCommand(string titulo, string descricao, DateTime dataInicio, DateTime dataFim, decimal metaFinanceira)
    {
        Titulo = titulo;
        Descricao = descricao;
        DataInicio = dataInicio;
        DataFim = dataFim;
        MetaFinanceira = metaFinanceira;
    }

    protected override IValidator GetValidator()
    {
        return new CriarCampanhaCommandValidator();
    }
}

public class CriarCampanhaCommandValidator : AbstractValidator<CriarCampanhaCommand>
{
    public CriarCampanhaCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória.");

        RuleFor(x => x.DataInicio)
            .NotEmpty().WithMessage("Data de início é obrigatória.");

        RuleFor(x => x.DataFim)
            .NotEmpty().WithMessage("Data de término é obrigatória.")
            .GreaterThan(x => x.DataInicio).WithMessage("Data de término deve ser posterior à data de início.");

        RuleFor(x => x.MetaFinanceira)
            .GreaterThan(0).WithMessage("Meta financeira deve ser maior que zero.");
    }
}
