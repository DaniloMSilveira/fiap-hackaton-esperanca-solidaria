using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EsperancaSolidaria.Application.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EsperancaSolidaria.Infraestructure.Security;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly string _audience;
    private readonly string _issuer;
    private readonly string _secretKey;
    private readonly int _expirationHours;

    public AutenticacaoService(IConfiguration configuration)
    {
        _audience = configuration["JwtSettings:Audience"] ?? throw new ArgumentNullException("JwtSettings:Audience");
        _issuer = configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer");
        _secretKey = configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey");
        _expirationHours = int.Parse(configuration["JwtSettings:ExpirationHours"] ?? "8");
    }

    public string GerarToken(Guid userId, string nomeCompleto, string email, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, nomeCompleto),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.Now.AddHours(_expirationHours),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Audience = _audience,
            Issuer = _issuer
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string CriptografarSenha(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public bool VerificarSenha(string senha, string senhaCriptografada)
    {
        return BCrypt.Net.BCrypt.Verify(senha, senhaCriptografada);
    }
}