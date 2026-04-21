using EsperancaSolidaria.BuildingBlocks.Queries;
using FluentValidation;

namespace EsperancaSolidaria.Application.Queries.Usuarios.Inputs;

public class ObterUsuarioPorIdQuery : Query<UsuarioResult>
{
    public Guid Id { get; set; }

    public ObterUsuarioPorIdQuery(Guid id)
    {
        Id = id;
    }

    protected override IValidator GetValidator()
    {
        return new ObterUsuarioPorIdQueryValidator();
    }
}

public class ObterUsuarioPorIdQueryValidator : AbstractValidator<ObterUsuarioPorIdQuery>
{
    public ObterUsuarioPorIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");
    }
}
