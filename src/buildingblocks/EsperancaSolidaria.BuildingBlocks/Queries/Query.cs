using FluentValidation;
using FluentValidation.Results;

namespace EsperancaSolidaria.BuildingBlocks.Queries;


public abstract class Query<TResult>
{
    /// <summary>
    /// Cada comando deve expor seu validator.
    /// </summary>
    protected abstract IValidator GetValidator();

    /// <summary>
    /// Executa a validação usando o validator definido no comando.
    /// </summary>
    public ValidationResult Validate()
    {
        var validator = GetValidator();
        var context = new ValidationContext<Query<TResult>>(this);
        return validator.Validate(context);
    }
}

