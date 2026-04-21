namespace EsperancaSolidaria.BuildingBlocks.Validators;

public class PasswordValidator
{
    public static bool StrongPasswordValidate(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        var hasUpperCase = password.Any(char.IsUpper);
        var hasLowerCase = password.Any(char.IsLower);
        var hasNumber = password.Any(char.IsDigit);
        var hasSpecialCharacter = password.Any(ch => "!@#$%^&*()_+[]{}|;:,.<>?".Contains(ch));

        return hasUpperCase && hasLowerCase && hasNumber && hasSpecialCharacter;
    }
}