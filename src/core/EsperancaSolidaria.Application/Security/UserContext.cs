using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace EsperancaSolidaria.Application.Security
{
    public class UserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? Id => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public string? Nome => User?.FindFirst(ClaimTypes.Name)?.Value;

        public List<string> Roles => User?
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new();

        public IEnumerable<Claim> Claims => User?.Claims ?? Enumerable.Empty<Claim>();
    }
}