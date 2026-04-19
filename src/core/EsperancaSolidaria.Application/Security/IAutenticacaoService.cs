namespace EsperancaSolidaria.Application.Security;

public interface IAutenticacaoService
{
    public string GerarToken(Guid userId, string nomeCompleto, string email, string role);
    public string CriptografarSenha(string senha);
    public bool VerificarSenha(string senha, string senhaCriptografada);
}