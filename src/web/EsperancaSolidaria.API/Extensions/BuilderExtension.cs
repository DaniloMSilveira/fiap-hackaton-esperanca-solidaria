using System.Reflection;
using OpenTelemetry.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EsperancaSolidaria.Infra.Security.Configurations;
using Microsoft.AspNetCore.Identity;
using EsperancaSolidaria.Infra.Security.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EsperancaSolidaria.Infra.Security.Contexts;
using EsperancaSolidaria.Infra.Data.Contexts;
using EsperancaSolidaria.Application.Services.Interfaces;
using EsperancaSolidaria.Application.Services;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infra.Data.Repositories;
using EsperancaSolidaria.Domain.Interfaces;
using EsperancaSolidaria.Infra.Data.UnitOfWork;
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
        builder.Services.AddDataContexts(builder.Configuration);
        builder.Services.AddServices(builder.Configuration);
        builder.Services.AddCustomSwagger();
        builder.Services.AddCustomMetrics();
    }

    private static IServiceCollection AddDataContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EsperancaSolidariaDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("EsperancaSolidaria")));

        // services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Application Services
        services.AddScoped<IUsuarioAppService, UsuarioAppService>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

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