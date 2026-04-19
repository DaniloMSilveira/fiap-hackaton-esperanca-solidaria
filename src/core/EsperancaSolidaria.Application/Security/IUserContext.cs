namespace EsperancaSolidaria.Application.Security;

public interface IUserContext
{
    public Guid? Id { get; }
    public string? Username { get; }
    public string? FullName { get; }
    public string? Email { get; }
    public IEnumerable<string> Roles { get; }
    public string? GetUserName();
    public UserProfile GetProfile();
}