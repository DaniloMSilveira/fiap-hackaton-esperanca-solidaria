using FluentValidation;

namespace EsperancaSolidaria.Application.Commands.Usuarios.Inputs;

public class RemoverUsuarioCommand : Command
{
    public Guid Id { get; private set; }
    public string Usuario { get; private set; } = string.Empty;

    public RemoverUsuarioCommand(Guid id, string usuario)
    {
        Id = id;
        Usuario = usuario;
    }

    protected override IValidator GetValidator()
    {
        return new RemoverUsuarioCommandValidator();
    }
}

public class RemoverUsuarioCommandValidator : AbstractValidator<RemoverUsuarioCommand>
{
    public RemoverUsuarioCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Id é obrigatório");

        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("Usuário é obrigatório");
    }
}