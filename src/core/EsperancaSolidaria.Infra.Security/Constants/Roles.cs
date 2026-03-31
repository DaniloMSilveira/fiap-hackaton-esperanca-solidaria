namespace EsperancaSolidaria.Infra.Security.Constants
{
    public class Roles
    {
        public const string DOADOR = nameof(DOADOR);
        public const string GESTOR_ONG = nameof(GESTOR_ONG);

        public static List<string> ObterListaRoles()
        {
            return new List<string>()
            {
                DOADOR,
                GESTOR_ONG
            };
        }
    }
}