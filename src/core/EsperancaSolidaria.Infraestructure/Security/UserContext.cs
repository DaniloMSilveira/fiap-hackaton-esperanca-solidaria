using System.Security.Claims;
using EsperancaSolidaria.Application.Security;
using Microsoft.AspNetCore.Http;

namespace EsperancaSolidaria.Infraestructure.Security;

/// <summary>
/// Contexto do usuário autenticado, extraindo informações das claims do JWT token.
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? Id
    {
        get
        {
            var claimValue = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(claimValue, out var id) ? id : null;
        }
    }

    public string? Username => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public string? FullName => _httpContextAccessor.HttpContext?.User?.Claims
        .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

    public string? Email => _httpContextAccessor.HttpContext?.User?.Claims
        .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.Claims
        .Where(c => c.Type == ClaimTypes.Role)
        .Select(c => c.Value) ?? Enumerable.Empty<string>();

    public string? GetUserName()
    {
        return Email;
    }

    public UserProfile GetProfile()
    {
        return new UserProfile(
            Id,
            FullName,
            Email,
            Roles
        );
    }
}