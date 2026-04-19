namespace EsperancaSolidaria.Application.Security;

public record UserProfile(
    Guid? Id,
    string? FullName,
    string? Email,
    IEnumerable<string> Roles
);
