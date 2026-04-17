using System.Text.RegularExpressions;
using EsperancaSolidaria.BuildingBlocks.Domain;

namespace EsperancaSolidaria.Domain.ValueObjects;

public class Cpf : IValueObject
{
    public string Value { get; }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CPF é obrigatório");

        var cleanCpf = Regex.Replace(value, @"\D", "");

        if (!IsValidCpf(cleanCpf))
            throw new ArgumentException("CPF inválido");

        Value = cleanCpf;
    }

    private static bool IsValidCpf(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        if (cpf.All(c => c == cpf[0]))
            return false;

        int[] digits = cpf.Select(c => int.Parse(c.ToString())).ToArray();

        // Validar primeiro dígito
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);

        int firstDigit = (sum * 10) % 11;
        if (firstDigit == 10) firstDigit = 0;

        if (digits[9] != firstDigit)
            return false;

        // Validar segundo dígito
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += digits[i] * (11 - i);

        int secondDigit = (sum * 10) % 11;
        if (secondDigit == 10) secondDigit = 0;

        return digits[10] == secondDigit;
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