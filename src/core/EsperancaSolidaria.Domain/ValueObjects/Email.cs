using System.Net.Mail;

namespace EsperancaSolidaria.Domain.ValueObjects;

public class Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email é obrigatório");

        if (!IsValidEmail(value))
            throw new ArgumentException("Email inválido");

        Value = value.ToLower();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email.ToLower();
        }
        catch
        {
            return false;
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is Email email && Value == email.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }
}