using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.BuildingBlocks.Validators;
using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Doacoes.Inputs;

public class CriarDoacaoCommand : Command
{
    public Guid CampanhaId { get; private set; }
    public Guid DoadorId { get; private set; }
    public decimal Valor { get; private set; }
    public string ReferenciaPagamento { get; private set; }

    public CriarDoacaoCommand(Guid campanhaId, decimal valor, string referenciaPagamento)
    {
        CampanhaId = campanhaId;
        Valor = valor;
        ReferenciaPagamento = referenciaPagamento;
    }

    public void PreencherDoadorId(Guid doadorId)
    {
        DoadorId = doadorId;
    }

    protected override IValidator GetValidator()
    {
        return new CriarDoacaoCommandValidator();
    }
}

public class CriarDoacaoCommandValidator : AbstractValidator<CriarDoacaoCommand>
{
    public CriarDoacaoCommandValidator()
    {
        RuleFor(x => x.CampanhaId)
            .NotEmpty().WithMessage("CampanhaId é obrigatório.");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor da doação deve ser maior que zero.");

        RuleFor(x => x.DoadorId)
            .NotEmpty().WithMessage("DoadorId é obrigatório.");

        RuleFor(x => x.ReferenciaPagamento)
            .NotEmpty().WithMessage("Referência de pagamento é obrigatória.")
            .MaximumLength(255).WithMessage("Referência de pagamento deve ter no máximo 255 caracteres.");
    }
}
