namespace EsperancaSolidaria.BuildingBlocks.Validators;

public class CPFValidator
{
    public static bool IsValidCpf(string cpf)
    {
        // Remover caracteres não numéricos
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

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
}