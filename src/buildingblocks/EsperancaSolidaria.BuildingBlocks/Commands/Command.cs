using FluentValidation;
using FluentValidation.Results;

public abstract class Command
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
        var context = new ValidationContext<Command>(this);
        return validator.Validate(context);
    }
}