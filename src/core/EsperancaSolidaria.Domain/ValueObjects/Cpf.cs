using System.Text.RegularExpressions;
using EsperancaSolidaria.BuildingBlocks.Domain;
using EsperancaSolidaria.BuildingBlocks.Validators;

namespace EsperancaSolidaria.Domain.ValueObjects;

public class Cpf : IValueObject
{
    public string Value { get; }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CPF é obrigatório");

        var cleanCpf = Regex.Replace(value, @"\D", "");

        if (!CPFValidator.IsValidCpf(cleanCpf))
            throw new ArgumentException("CPF inválido");

        Value = cleanCpf;
    }

    public override bool Equals(object? obj)
    {
        return obj is Cpf cpf && Value == cpf.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Value.Substring(0, 3)}.{Value.Substring(3, 3)}.{Value.Substring(6, 3)}-{Value.Substring(9)}";
    }
}