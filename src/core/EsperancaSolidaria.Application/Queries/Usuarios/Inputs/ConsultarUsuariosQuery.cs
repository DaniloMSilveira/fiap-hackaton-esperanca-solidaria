using EsperancaSolidaria.BuildingBlocks.Queries;
using FluentValidation;

namespace EsperancaSolidaria.Application.Queries.Usuarios.Inputs;

public class ConsultarUsuariosQuery : Query<PaginatedResult<UsuarioListaResult>>
{
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
    public string? Nome { get; set; }
    public string? Email { get; set; }

    public ConsultarUsuariosQuery(int pagina, int tamanhoPagina, string? nome = null, string? email = null)
    {
        Pagina = pagina;
        TamanhoPagina = tamanhoPagina;
        Nome = nome;
        Email = email;
    }

    protected override IValidator GetValidator()
    {
        return new ConsultarUsuariosQueryValidator();
    }
}

public class ConsultarUsuariosQueryValidator : AbstractValidator<ConsultarUsuariosQuery>
{
    public ConsultarUsuariosQueryValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThan(0)
            .WithMessage("Página deve ser maior que 0");

        RuleFor(x => x.TamanhoPagina)
            .GreaterThan(0)
            .WithMessage("Tamanho da página deve ser maior que 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Tamanho máximo da página é 100 itens");
    }
}
