using EsperancaSolidaria.BuildingBlocks.Messaging;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infraestructure.Messaging;
using EsperancaSolidaria.Infraestructure.Persistence.Contexts;
using EsperancaSolidaria.Infraestructure.Persistence.Repositories;
using EsperancaSolidaria.Infraestructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace EsperancaSolidaria.Worker.DoacaoRealizada.Extensions;

public static class BuilderExtension
{
    public static void Configure(HostApplicationBuilder builder)
    {
        builder.Services.AddMessageBus(builder.Configuration);
        builder.Services.AddDataContexts(builder.Configuration, builder.Environment);
        builder.Services.AddServices(builder.Configuration);
    }

    private static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMqOptions"));

        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();

        return services;
    }

    private static IServiceCollection AddDataContexts(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext<EsperancaSolidariaDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("EsperancaSolidaria"));
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<ICampanhaRepository, CampanhaRepository>();

        // Worker
        services.AddHostedService<DoacaoRealizadaWorker>();

        return services;
    }
}