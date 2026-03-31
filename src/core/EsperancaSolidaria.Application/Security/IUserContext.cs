using System.Security.Claims;

namespace EsperancaSolidaria.Application.Security
{
    public interface IUserContext
    {
        public string? Id { get; }
        public string? Email { get; }
        public string? Nome { get; }
        public List<string> Roles { get; }
        public IEnumerable<Claim> Claims { get; }
    }
}