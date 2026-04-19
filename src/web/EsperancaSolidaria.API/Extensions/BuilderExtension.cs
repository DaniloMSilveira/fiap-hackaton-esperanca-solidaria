using System.Reflection;
using OpenTelemetry.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EsperancaSolidaria.Infraestructure.Persistence.Contexts;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infraestructure.Persistence.Repositories;
using EsperancaSolidaria.Infraestructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.BuildingBlocks.Events;
using EsperancaSolidaria.Application.Security;
using EsperancaSolidaria.Infraestructure.Security;
using EsperancaSolidaria.Application.Commands.Autenticacao.Handlers;

namespace EsperancaSolidaria.API.Extensions;

public static class BuilderExtension
{
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAuthentication(builder.Configuration);
        builder.Services.AddDataContexts(builder.Configuration, builder.Environment);
        builder.Services.AddServices(builder.Configuration);
        builder.Services.AddCustomSwagger();
        builder.Services.AddCustomMetrics();
    }

    private static IServiceCollection AddDataContexts(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
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
        // Command Handlers
        services.AddScoped<IAutenticacaoCommandHandler, AutenticacaoCommandHandler>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // Domain Events
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Security
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        return services;
    }

    private static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    private static IServiceCollection AddCustomMetrics(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddPrometheusExporter();
            });

        return services;
    }

    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured.");

        // JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    // // Logs
    // builder.Services.AddSerilog(new LoggerConfiguration()
    //     .WriteTo.Console()
    //     .WriteTo.GrafanaLoki(
    //         builder.Configuration["Loki:Uri"]!,
    //         new List<LokiLabel>()
    //         {
    //             new()
    //             {
    //                 Key = "service_name",
    //                 Value = OpenTelemetryExtensions.ServiceName
    //             },
    //             new()
    //             {
    //                 Key = "using_database",
    //                 Value = "true"
    //             }
    //         })
    //     .Enrich.WithSpan(new SpanOptions() { IncludeOperationName = true, IncludeTags = true })
    //     .CreateLogger());

    // // Metrics and Tracing
    // builder.Services.AddOpenTelemetry()
    //     .WithMetrics(builder =>
    //     {
    //         builder
    //             .AddAspNetCoreInstrumentation()
    //             .AddHttpClientInstrumentation()
    //             .AddPrometheusExporter();
    //     })
    //     .WithTracing((traceBuilder) =>
    //     {
    //         traceBuilder
    //             .AddSource(OpenTelemetryExtensions.ServiceName)
    //             .SetResourceBuilder(
    //                 ResourceBuilder.CreateDefault()
    //                     .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
    //                         serviceVersion: OpenTelemetryExtensions.ServiceVersion))
    //             .AddAspNetCoreInstrumentation()
    //             .AddSqlClientInstrumentation()
    //             .AddOtlpExporter()
    //             .AddConsoleExporter();
    //     });
}