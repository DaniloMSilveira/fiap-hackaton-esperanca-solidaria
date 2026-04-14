using FluentValidation.Results;

namespace EsperancaSolidaria.Application.DTO.Outputs;

public class BaseOutput
{
    public bool Success { get; private set; }
    public List<string> Errors { get; private set; } = [];

    private BaseOutput(bool success, List<string>? errors = null)
    {
        Success = success;
        Errors = errors ?? new();
    }

    public static BaseOutput Ok()
        => new(true, null);

    public static BaseOutput Fail(string error)
        => new(false, new List<string> { error });

    public static BaseOutput Fail(List<string> errors)
        => new(false, errors);

    public static BaseOutput Fail(ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .Select(e => e.ErrorMessage)
            .ToList();

        return new(false, errors);
    }
}

public class BaseOutput<T>
{
    public bool Success { get; private set; }
    public List<string> Errors { get; private set; } = [];
    public T? Data { get; private set; }

    private BaseOutput(bool success, List<string>? errors = null, T? data = default)
    {
        Success = success;
        Errors = errors ?? new();
        Data = data;
    }

    public static BaseOutput<T> Ok()
        => new(true, null);

    public static BaseOutput<T> Ok(T data)
        => new(true, null, data);

    public static BaseOutput<T> Fail(string error)
        => new(false, new List<string> { error });

    public static BaseOutput<T> Fail(List<string> errors)
        => new(false, errors);

    public static BaseOutput<T> Fail(ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .Select(e => e.ErrorMessage)
            .ToList();

        return new(false, errors);
    }
}