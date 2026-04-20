using EsperancaSolidaria.Application.Security;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Domain.ValueObjects;

namespace EsperancaSolidaria.API.Extensions;

public static class SeedDataExtension
{
    public static async Task SeedAdminUserAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SeedDataExtension");
        var usuarioRepository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var autenticacaoService = scope.ServiceProvider.GetRequiredService<IAutenticacaoService>();

        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("SeedAdmin configuration is missing. Skipping admin user seed.");
            return;
        }

        var existingUser = await usuarioRepository.ObterPorEmailAsync(email);
        if (existingUser is not null)
        {
            logger.LogInformation("Admin user already exists: {Email}", email);
            return;
        }

        var cpf = new Cpf("52998224725");
        var senhaCriptografada = autenticacaoService.CriptografarSenha(password);

        var administrator = new Usuario(
            nomeCompleto: "Gestor",
            email: new Email(email),
            cpf: cpf,
            senhaCriptografada: senhaCriptografada,
            perfilAcesso: EPerfilAcesso.GestorONG,
            usuario: "SeedAdmin"
        );

        usuarioRepository.Adicionar(administrator);

        var (isSuccess, errorMessage) = await unitOfWork.SaveChangesAsync();
        if (!isSuccess)
        {
            logger.LogError("Failed to seed admin user: {Email}. Error: {ErrorMessage}", email, errorMessage);
            return;
        }

        logger.LogInformation("Seed admin user created successfully: {Email}", email);
    }
}
