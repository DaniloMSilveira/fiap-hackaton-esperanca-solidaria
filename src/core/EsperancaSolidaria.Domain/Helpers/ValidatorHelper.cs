namespace EsperancaSolidaria.Domain.Helpers;

public class ValidatorHelper
{
    public static bool ValidarSenhaForte(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
            return false;

        var temCaractereMaiusculo = senha.Any(char.IsUpper);
        var temCaractereMinusculo = senha.Any(char.IsLower);
        var temNumero = senha.Any(char.IsDigit);
        var temCaractereEspecial = senha.Any(ch => "!@#$%^&*()_+[]{}|;:,.<>?".Contains(ch));

        return temCaractereMaiusculo && temCaractereMinusculo && temNumero && temCaractereEspecial;
    }
}